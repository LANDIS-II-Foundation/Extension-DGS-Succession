using System;
using System.Collections.Generic;
using System.Text;
using Landis.Extension.ShawDamm;
using Landis.Extension.GiplDamm;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.Climate;

namespace Landis.Extension.Succession.DGS
{
    public class TempHydroUnit
    {
        #region fields

        private bool _firstMonthHasRun;
        private static int[] _firstDayOfMonth = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 };  // zero-based.  note: without leap year
        private static int[] _lengthOfMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };  // note: without leap year

        #endregion

        #region constructor

        public TempHydroUnit(string name, string climateRegionName)
        {
            ClimateRegion = PlugIn.ModelCore.Ecoregions[climateRegionName];

            // **
            // set up Shaw instance
            var shawInputFilePath = GetShawInputFilePath();

            ShawInstance = new ShawDamm.ShawDamm();
            if (!ShawDamm.ShawDamm.HasGlobalSetup)
                ShawInstance.GlobalInitialization(shawInputFilePath);

            if (!ShawInstance.Initialize(name))
                return;

            // get Shaw depths
            ShawDepths = ShawInstance.GetDepths();

            // read and
            // set the initial soil moisture from the first line of the soil moisture file
            var soilMoistureData = File.ReadAllLines(ShawInstance.SoilMoistureFile).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            var t = ParseLine(soilMoistureData.First());
            t = t.GetRange(3, t.Count - 3);    // remove day-hr-time

            if (t.Count != ShawDepths.Count)
                throw new ApplicationException($"The number of initial Shaw moisture points {t.Count} for THU {Name} does not equal the number of Shaw depth points {ShawDepths.Count}");

            var soilMoistureInit = t.Select(x => double.Parse(x)).ToArray();

            ShawInstance.SetInitialSoilMoisture(soilMoistureInit);


            // **
            // set up Gipl instance
            var giplInputFilePath = GetGiplInputPath();

            // setup global properties inputs
            if (!GiplDamm.GiplDamm.HasGlobalSetup)
                GiplDamm.GiplDamm.GlobalInitialization(giplInputFilePath);

            GiplInstance = new GiplDamm.GiplDamm();
            if (!GiplInstance.Initialize(giplInputFilePath, name, ShawDepths))
                return;

            // get Gipl depths
            GiplDepths = GiplInstance.GetGiplDepths();

            // set up monthly result arrays
            MonthlyShawDammResults = new ShawDammResults[12];
            MonthlyGiplDammResults = new GiplDammResults[12];

            // set up depth increments at Shaw and Gipl depth points because the points may not be evenly spaced
            ShawDepthIncrements = new List<double>();
            for (var i = 0; i < ShawDepths.Count - 1; ++i)
                ShawDepthIncrements.Add(ShawDepths[i + 1] - ShawDepths[i]);

            GiplDepthIncrements = new List<double>();
            for (var i = 0; i < GiplDepths.Count - 1; ++i)
                GiplDepthIncrements.Add(GiplDepths[i + 1] - GiplDepths[i]);

            // initialize monthly species records
            MonthlySpeciesRecords = new Dictionary<ISpecies, ThuSpeciesRecord>[12];
            for (var i = 0; i < 12; i++)
            {
                MonthlySpeciesRecords[i] = new Dictionary<ISpecies, ThuSpeciesRecord>();
            }
        }

        #endregion

        #region properties

        public string Name { get; set; }
        public IEcoregion ClimateRegion { get; set; }

        public ShawDamm.ShawDamm ShawInstance { get; }
        public GiplDamm.GiplDamm GiplInstance { get; }

        public ShawDammResults[] MonthlyShawDammResults { get; }
        public GiplDammResults[] MonthlyGiplDammResults { get; }

        public List<double> ShawDepths { get; }
        public List<double> ShawDepthIncrements { get; }

        public List<double> GiplDepths { get; }
        public List<double> GiplDepthIncrements { get; }

        public Dictionary<ISpecies, ThuSpeciesRecord>[] MonthlySpeciesRecords { get; }
        
        #endregion

        #region methods

        public void RunForYear(int year, AnnualClimate_Daily dailyWeather)
        {
            // convert weather data to lists so each month can be grabbed
            var precips = dailyWeather.DailyPrecip.ToList();
            var tmaxs = dailyWeather.DailyMaxTemp.ToList();
            var tavgs = dailyWeather.DailyTemp.ToList();
            var tmins = dailyWeather.DailyMinTemp.ToList();
            var winds = dailyWeather.DailyWindSpeed.ToList();
            var solars = dailyWeather.DailyShortWaveRadiation.ToList();
            var tdews = dailyWeather.DailyTemp.ToList();        // todo: change this when Tdew is available

            for (var month = 0; month < 12; ++month)
            {
                // get Landis weather data for the month
                int startingDay, dayCount;
                GetDailyWeatherRangeForMonth(month, year, out startingDay, out dayCount);

                var precip = precips.GetRange(startingDay, dayCount);
                var tmax = tmaxs.GetRange(startingDay, dayCount);
                var tavg = tavgs.GetRange(startingDay, dayCount);
                var tmin = tmins.GetRange(startingDay, dayCount);
                var wind = winds.GetRange(startingDay, dayCount);
                var solar = solars.GetRange(startingDay, dayCount);
                var tdew = tdews.GetRange(startingDay, dayCount);

                var lastMonth = month == 0 ? 11 : month - 1;

                // **
                // Gipl

                // estimate snow depth for the month based on precip, air temperature, and the snow depth and density
                //  from Shaw results at the end of the previous month.
                // if this is the first month of the simulation (which would be a January), Shaw results are not available, so run the process
                //  from Sep. 1 to Dec. 31 using this year's weather data and use the final snow depth and density on Dec. 31 
                //  as the starting conditions for the January calculation.

                double[] dailySnowDepthEstimate;
                double snowThermalConductivity, snowVolumetricHeatCapacity;
                double initialSnowDepth, initialSnowDensity;
                double dummy;

                if (_firstMonthHasRun)
                {
                    initialSnowDepth = MonthlyShawDammResults[lastMonth].DailySnowThickness.Last();
                    initialSnowDensity = MonthlyShawDammResults[lastMonth].DailySnowDensity.Last();
                }
                else
                {
                    var sepStart = _firstDayOfMonth[8] + (IsLeapYear(year) ? 1 : 0);
                    var daysToEndOfYear = 122;  // days in Sep, Oct, Nov, and Dec

                    EstimateDailySnowDepth(precips.GetRange(sepStart, daysToEndOfYear), tavgs.GetRange(sepStart, daysToEndOfYear), 0.0, 0.0, 
                        out initialSnowDepth, out initialSnowDensity, out dummy, out dummy);

                    _firstMonthHasRun = true;
                }

                dailySnowDepthEstimate = EstimateDailySnowDepth(precip, tavg, initialSnowDepth, initialSnowDensity, out dummy, out dummy, out snowThermalConductivity, out snowVolumetricHeatCapacity);

                // call Gipl
                //  pass the daily air temperature, estimates of daily snow depth, snow thermal conductivity, and snow volumetric heat capacity, and Shaw's soil moisture profile at the end
                //  of the previous month.
                //  if this is the first month, there will be no Shaw results, so this will be passed as null, in which case Gipl will use the default profile from its properties file. 
                var giplResults = MonthlyGiplDammResults[month] = GiplInstance.CalculateSoilTemperature(tavg.ToArray(), dailySnowDepthEstimate, MonthlyShawDammResults[lastMonth]?.DailySoilMoistureProfiles.Last(), snowThermalConductivity, snowVolumetricHeatCapacity);


                // **
                // Shaw

                var shawWeatherData = new ShawDammDailyWeatherRecord[dayCount];
                for (var i = 0; i < dayCount; ++i)
                {
                    var weather = new ShawDammDailyWeatherRecord { Precip = precip[i], Tmax = tmax[i], Tmin = tmin[i], Tdew = tdew[i], Solar = solar[i], Wind = wind[i] };

                    // conversions from Landis weather to Shaw weather
                    weather.Precip *= 10.0;     // cm to mm
                    if (weather.Wind < 0.0)
                        weather.Wind = 0.0;     // missing wind data
                    else
                        weather.Wind /= 3.6;    // km/hr to m/s

                    shawWeatherData[i] = weather;
                }

                // call Shaw
                //  pass the Shaw weather data and the soil temperature profiles from this month's Gipl run.
                //  also pass an average temperature in case Gipl is not used.  todo: this is not currently enabled in the code.
                // Shaw will return the daily soil moisture profiles, daily snow thickness, daily snow heat capacity, and daily snow volumetric heat capacity.
                //  currently the snow heat capacity and volumetric heat capacity are not used.
                var shawResults = MonthlyShawDammResults[month] = ShawInstance.CalculateSoilResults(year, startingDay, shawWeatherData, MonthlyGiplDammResults[month].DailySoilTemperatureProfilesAtShawDepths, -1.0);

                // calculate species records for this month based on gipl and shaw results
                foreach (var species in PlugIn.ModelCore.Species)
                {
                    var hasAdventRoots = SpeciesData.AdventRoots[species];
                    var rootingdepth = SpeciesData.RootingDepth[species] / 100.0;   // convert rooting depth to meters
                    var startingDepth = hasAdventRoots ? 0.0 : SpeciesData.AdventitiousLayerDepth;

                    // average Gipl's soil temperature profile (at Shaw depths) to get the soil temperature.
                    //  start the average at either the top of the profile (if the species has adventitious roots), 
                    //  or at the bottom of the adventitious layer (if the species does not have adventitious roots).
                    //  end the average at the rooting depth for the species.

                    var soilTemperature = AverageOrIntegrateOverProfile(true, giplResults.AverageSoilTemperatureProfileAtShawDepths, startingDepth, rootingdepth);
                    var temperatureLimit = CohortBiomass.TemperatureLimitEquation(soilTemperature, species);

                    // integrate Shaw's soil moisture profile (at Shaw depths) to get the availableWater.
                    //  start the average at either the top of the profile (if the species has adventitious roots), 
                    //  or at the bottom of the adventitious layer (if the species does not have adventitious roots).
                    //  end the average at the rooting depth for the species.

                    var availableWater = AverageOrIntegrateOverProfile(false, shawResults.MonthSoilMoistureProfile, startingDepth, rootingdepth);
                    var waterLimit = CohortBiomass.WaterLimitEquation(availableWater, species);

                    MonthlySpeciesRecords[month][species] = new ThuSpeciesRecord { SoilTemperature = soilTemperature, TemperatureLimit = temperatureLimit, AvailableWater = availableWater, WaterLimit = waterLimit };
                }
            }
        }

        #endregion

        #region private methods

        private static string GetShawInputFilePath()
        {
            //return @"C:\Users\mslucash\Dropbox\SHAW\POC 10.10.2019\POC.inp"; //John's path
            return @"C:\Users\lucash\Dropbox\SHAW\UP1A_Birch\UP1A.inp"; //Mel's path
            //return @"D:\Shelbys Files\AK_DGS_Runs\SHAW\POC 10.10.2019\POC.inp"; //Shelby's path
            //return @"C:\Users\mslucash\Documents\John\SHAW\TestCases\POC\POC.inp";

            Console.WriteLine();
            Console.WriteLine(">>>>>>> Simultaneous Heat And Water (SHAW) Model <<<<<<<");
            Console.WriteLine("                    Version 3.0.1");
            Console.WriteLine("Enter the file path containing the list of input/output files:");

            return Console.ReadLine() ?? string.Empty;
        }
        
        private static string GetGiplInputPath()
        {
            //return @"C:\Users\mslucash\Dropbox\GIPL\POC 10.10.2019";  // John's path
            return @"C:\Users\lucash\Dropbox\GIPL\POC 10.10.2019";  // Mel's path
            //return @"D:\Shelbys Files\AK_DGS_Runs\GIPL\POC 10.10.2019";  // Shelby's path
            //return @"C:\Users\mslucash\Documents\John\GIPL\TestCases\POC";

            Console.WriteLine();
            Console.WriteLine(">>>>>>> GIPL DAMM <<<<<<<");
            Console.WriteLine("Enter the directory path containing the list of input files:");

            return Console.ReadLine() ?? string.Empty;
        }

        private static void GetDailyWeatherRangeForMonth(int month, int year, out int startingDay, out int dayCount)
        {
            startingDay = _firstDayOfMonth[month];
            dayCount = _lengthOfMonth[month];

            if (IsLeapYear(year))
            {
                if (month > 1) ++startingDay;
                if (month == 1) ++dayCount;
            }
        }

        private static bool IsLeapYear(int year)
        {
            if (year % 400 == 0)
                return true;

            if (year % 100 == 0)
                return false;

            return year % 4 == 0;
        }
 
        private double[] EstimateDailySnowDepth(List<double> dailyPrecipitation, List<double> dailyAirTemperature, double initialSnowDepth, double initialSnowDensity, 
            out double finalSnowDepth, out double finalSnowDensity, out double thermalConductivity, out double volumetricHeatCapacity)
        {
            // based on a MATLAB script from Dmitry Nicolsky
            // units: 
            //  dailyPrecipitation              m/day
            //  dailyAirTemperature             C
            //  initial/final SnowDepth         m
            //  initial/final SnowDensity       kg/m3
            //  thermalConductivity             W/m/C
            //  volumetricHeatCapacity          J/m3/K

            // fixed constants:
            const double freshSnowDensity = 100.0;    // kg/m3
            const double waterDensity = 1000.0;        // kg/m3
            const double gravity = 9.81;                // m/s2
            const double snowPackMelting = 0.8;

            const double timeStep = 3600.0 * 24.0;            // s - number of seconds in one day
            const double C1 = 0.01e-2 / 3600;
            const double C2 = 21.0;

            var dailySnowDepth = new double[dailyPrecipitation.Count];
            var snowThermalConductivities = new List<double>();
            var snowVolumetricHeatCapacities = new List<double>();

            var snowDepth = initialSnowDepth;
            var snowDensity = initialSnowDensity;

            for (var day = 0; day < dailyPrecipitation.Count; ++day)
            {
                double precip;
                if (dailyAirTemperature[day] < 0.0)
                    precip = dailyPrecipitation[day];
                else
                {
                    precip = 0.0;
                    snowDepth = snowDepth * snowPackMelting;
                }

                if (snowDepth + precip > 0.0)
                {
                    // Manage new snow precipitation
                    snowDensity = (snowDepth * snowDensity + precip * freshSnowDensity) / (snowDepth + precip);
                    snowDepth = snowDepth + precip;
                    var swe = snowDensity * snowDepth;

                    // Snow compaction
                    var snowWeight = snowDensity * snowDepth / 2.0 * gravity;
                    snowDensity = snowDensity / Math.Max(1.0 - timeStep * snowWeight * C1 * Math.Exp(-C2 * snowDensity / waterDensity), 0.5);
                    snowDepth = swe / snowDensity;
                }

                dailySnowDepth[day] = snowDepth;

                // accumulate thermal conductivities and volumetric heat capacities if snow is present
                if (snowDepth > 0.0)
                {
                    var tK = 0.021 + 2.51 * Math.Pow(snowDensity / waterDensity, 2.0);
                    var specificHeatC = 92.96 + 7.37 * (dailyAirTemperature[day] / 2 + 273.15);
                    var hC = specificHeatC * snowDensity;

                    snowThermalConductivities.Add(tK);
                    snowVolumetricHeatCapacities.Add(hC);
                }
            }

            finalSnowDepth = snowDepth;
            finalSnowDensity = snowDensity;

            thermalConductivity = snowThermalConductivities.Any() ? snowThermalConductivities.Average() : 0.0;
            volumetricHeatCapacity = snowVolumetricHeatCapacities.Any() ? snowVolumetricHeatCapacities.Average() : 0.0;

            return dailySnowDepth;
        }

        private double AverageOrIntegrateOverProfile(bool makeAverage, double[] profile, double startingDepth, double endingDepth)
        {
            // assumes the depths start at 0.0.

            var weight = 0.0;
            var sum = 0.0;

            var i0 = 0;
            if (startingDepth > 0.0)
            {
                // find the first depth point that exceeds startingDepth
                i0 = ShawDepths.FindIndex(x => x > startingDepth);
                if (i0 < 0)
                    throw new ApplicationException($"Error: CohortBiomass.AverageOverProfile(): starting depth {startingDepth} not within the profile range.");

                // add the profile value at i0 - 1, weighted by the depth between the starting depth and the depth at i0.
                sum += profile[i0 - 1] * (ShawDepths[i0] - startingDepth);
                weight += ShawDepths[i0] - startingDepth;
            }

            // find the last depth point before ending depth
            var i1 = ShawDepths.FindIndex(x => x > endingDepth);
            i1 = i1 < 0 ? ShawDepths.Count - 1 : i1 - 1;    // if endingDepth is below the last depth, use the last depth point and assume the profile extends to the ending depth

            // add the profile at i1, weighted by the depth between the depth at i1 and the ending depth.
            sum += profile[i1] * (endingDepth - ShawDepths[i1]);
            weight += endingDepth - ShawDepths[i1];

            // now add all the points from i0 to i1 - 1, weighted by their depth increments
            for (var i = i0; i < i1; ++i)
            {
                sum += profile[i] * ShawDepthIncrements[i];
                weight += ShawDepthIncrements[i];
            }

            return makeAverage ? sum / weight : sum;
        }

        private static Regex _whiteSpaceRegex = new Regex(@"\s+");
        private static Regex _repeatInputRegex = new Regex(@"^(?'count'\d+)\*(?'value'.*)$");

        private static List<string> ParseLine(string s)
        {
            // I'm not entirely sure whether FORTRAN allows the repeat format, i.e. count*value, anywhere within whitespace-delimited text, but let's assume it does.
            var atoms = _whiteSpaceRegex.Split(s.Trim()).ToList();

            var t1 = new List<string>(atoms.Count);
            Match m;
            foreach (var a in atoms)
            {
                if ((m = _repeatInputRegex.Match(a)).Success)
                {
                    t1.AddRange(Enumerable.Repeat(m.Groups["value"].Value, int.Parse(m.Groups["count"].Value)));
                }
                else
                {
                    t1.Add(a);
                }
            }
            return t1;
        }
        #endregion
    }

    public class ThuSpeciesRecord
    {
        public double AvailableWater { get; set; }
        public double WaterLimit { get; set; }
        public double SoilTemperature { get; set; }
        public double TemperatureLimit { get; set; }
    }
}
