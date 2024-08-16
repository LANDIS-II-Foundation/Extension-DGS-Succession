//  Authors: John McNabb, Melissa Lucash, Robert Scheller

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;

using Landis.Library.InitialCommunities.Universal;
using Landis.Library.Succession;
using Landis.Library.UniversalCohorts;
using Landis.Library.Climate;
using Landis.Library.Metadata;

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Landis.Extension.ShawDamm;
using Landis.Extension.GiplDamm;
using System.Threading.Tasks;

namespace Landis.Extension.Succession.DGS
{
    public class PlugIn
        : Landis.Library.Succession.ExtensionBase
    {
        public static readonly string ExtensionName = "DGS Succession";
        private static ICore modelCore;
        public static IInputParameters Parameters;
        public static double[] ShadeLAI;
        public static double AnnualWaterBalance;

        private List<ISufficientLight> sufficientLight;
        public static string SoilCarbonMapNames = null;
        public static int SoilCarbonMapFrequency;
        public static string SoilNitrogenMapNames = null;
        public static int SoilNitrogenMapFrequency;
        public static string ANPPMapNames = null;
        public static int ANPPMapFrequency;
        public static string ANEEMapNames = null;
        public static int ANEEMapFrequency;
        public static string TotalCMapNames = null;
        public static int TotalCMapFrequency;
        public static string InputCommunityMapNames = null;
        public static int InputCommunityMapFrequency;
        public static int SuccessionTimeStep;
        public static double ProbEstablishAdjust;

        //public static int FutureClimateBaseYear;
        public static int B_MAX;
        private ICommunity initialCommunity;

        public static int[] SpeciesByPlant;
        public static int[] SpeciesBySerotiny;
        public static int[] SpeciesByResprout;
        public static int[] SpeciesBySeed;

        //public static Dictionary<IEcoregion, TempHydroUnit> TempHydroUnits { get; set; }

        //public static TempHydroUnit TempHydroUnit { get; set; }
        public static List<TempHydroUnit> TempHydroUnits { get; set; }
        //public static Dictionary<ActiveSite, TempHydroUnit> TemHydroUnitForSite { get; set; }

        public static bool ShawGiplEnabled { get; set; }

        private uint? prevSiteDataIndex;

        //private List<TempHydroUnit> _tempHydroUnits;

        //public static int B_MAX;

        //---------------------------------------------------------------------

        public PlugIn()
            : base(ExtensionName)
        {
        }

        //---------------------------------------------------------------------

        public override void LoadParameters(string dataFile,
                                            ICore mCore)
        {
            modelCore = mCore;
            SiteVars.Initialize();
            InputParametersParser parser = new InputParametersParser();
            Parameters = Data.Load<IInputParameters>(dataFile, parser);

        }

        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }


        //---------------------------------------------------------------------

        public override void AddCohortData()
        {
            dynamic tempObject = additionalCohortParameters;
            tempObject.WoodBiomass = 0.0f;
            tempObject.LeafBiomass = 0.0f;
        }

        //---------------------------------------------------------------------

        public override void Initialize()
        {
            //Console.WriteLine();
            Console.WriteLine("Attach process to Visual Studio for debugging and hit return");
            Console.ReadLine();     // JM:  added for debugging.  use this to stop the run to allow me to attach visual studio to the dotnet process

            ModelCore.UI.WriteLine("Initializing {0} ...", ExtensionName);
            Timestep              = Parameters.Timestep;
            SuccessionTimeStep    = Timestep;
            sufficientLight       = Parameters.LightClassProbabilities;
            ProbEstablishAdjust = Parameters.ProbEstablishAdjustment;
            MetadataHandler.InitializeMetadata(Timestep, modelCore, SoilCarbonMapNames, SoilNitrogenMapNames, ANPPMapNames, ANEEMapNames, TotalCMapNames); //,LAIMapNames, ShadeClassMapNames);
            
            FunctionalType.Initialize(Parameters);
            SpeciesData.Initialize(Parameters);
            ReadMaps.ReadSoilDepthMap(Parameters.SoilDepthMapName);
            ReadMaps.ReadSoilDrainMap(Parameters.SoilDrainMapName);
            ReadMaps.ReadSoilBaseFlowMap(Parameters.SoilBaseFlowMapName);
            ReadMaps.ReadSoilStormFlowMap(Parameters.SoilStormFlowMapName);
            ReadMaps.ReadFieldCapacityMap(Parameters.SoilFieldCapacityMapName);
            ReadMaps.ReadWiltingPointMap(Parameters.SoilWiltingPointMapName);
            ReadMaps.ReadPercentSandMap(Parameters.SoilPercentSandMapName);
            ReadMaps.ReadPercentClayMap(Parameters.SoilPercentClayMapName);
            ReadMaps.ReadSoilBulkDensityMap(Parameters.SoilBulkDensityMapName);
            ReadMaps.ReadSoilParticleDensityMap(Parameters.SoilParticleDensityMapName);
            //Util.ReadDoubleMap(Parameters.SoilParticleDensityMapName);
            //ReadMaps.ReadSoilCMap(Parameters.InitialSOC_PrimaryMapName);
            //ReadMaps.ReadSoilNMap(Parameters.InitialSON_PrimaryMapName);
            ReadMaps.ReadSoilCNMaps(Parameters.InitialSOC_PrimaryMapName, Parameters.InitialSON_PrimaryMapName);
            ReadMaps.ReadDeadWoodMaps(Parameters.InitialDeadSurfaceMapName, Parameters.InitialDeadSoilMapName);

            //Initialize climate.
            Climate.Initialize(Parameters.ClimateConfigFile, false, modelCore);
            //FutureClimateBaseYear = Climate.Future_MonthlyData.Keys.Min();
            ClimateRegionData.Initialize(Parameters);
            
            ShadeLAI = Parameters.MaximumShadeLAI; 
            OtherData.Initialize(Parameters);
            FireEffects.Initialize(Parameters);

            //  Cohorts must be created before the base class is initialized
            //  because the base class' reproduction module uses the core's
            //  SuccessionCohorts property in its Initialization method.
            Landis.Library.UniversalCohorts.Cohorts.Initialize(Timestep, new CohortBiomass());

            // Initialize Reproduction routines:
            Reproduction.SufficientResources = SufficientLight;
            Reproduction.Establish = Establish;
            Reproduction.AddNewCohort = AddNewCohort;
            Reproduction.MaturePresent = MaturePresent;
            Initialize(modelCore, Parameters.SeedAlgorithm);

            Cohort.MortalityEvent += CohortMortality;

            InitializeSites(Parameters.InitialCommunities, Parameters.InitialCommunitiesMap, modelCore);

            // initialize Shaw and Gipl
            ShawGiplEnabled = true;
            InitializeTempHydroUnits();

            //if (Parameters.CalibrateMode)
                //Outputs.CreateCalibrateLogFile();
            //Establishment.InitializeLogFile();

            //B_MAX = 0;
            //foreach (ISpecies species in ModelCore.Species)
            //{
            //    if (SpeciesData.Max_Biomass[species] > B_MAX)
            //        B_MAX = SpeciesData.Max_Biomass[species];
            //}

            foreach (ActiveSite site in ModelCore.Landscape)
            {
                Main.ComputeTotalCohortCN(site, SiteVars.Cohorts[site]);
                SiteVars.FineFuels[site] = (SiteVars.SurfaceStructural[site].Carbon + SiteVars.SurfaceMetabolic[site].Carbon) * 2.0;
            }
            Outputs.WritePrimaryLogFile(0);
            Outputs.WriteShortPrimaryLogFile(0);
            
            //Main.timer1 = new System.Diagnostics.Stopwatch();
            //Main.soilLayerTimer = new System.Diagnostics.Stopwatch();

            //Main.SiteMonth = new int[ModelCore.Landscape.ActiveSiteCount + 1];
            //Main.SiteMonthCnt = new int[ModelCore.Landscape.ActiveSiteCount + 1];
        }

        //---------------------------------------------------------------------

        public override void Run()
        {

            if (ModelCore.CurrentTime > 0)
                    SiteVars.InitializeDisturbances();

            ClimateRegionData.AnnualNDeposition = new Landis.Library.Parameters.Ecoregions.AuxParm<double>(ModelCore.Ecoregions);
            SpeciesByPlant = new int[ModelCore.Species.Count];
            SpeciesByResprout = new int[ModelCore.Species.Count];
            SpeciesBySerotiny = new int[ModelCore.Species.Count];
            SpeciesBySeed = new int[ModelCore.Species.Count];

            if (ShawGiplEnabled)
            {
                AssignTempHydroUnits();
                RunTempHydroUnits();
            }

            //base.RunReproductionFirst();
            //Main.siteCounter = 0;
            //Main.soilLayerTimer.Reset();
            //Main.timer1.Reset();

            ModelCore.UI.WriteLine("ExtensionBaseRun: Start");         
            base.Run();
            //ExtensionBaseRun();
            ModelCore.UI.WriteLine("ExtensionBaseRun: Stop");
            //ModelCore.UI.WriteLine($"Main.Run Total ElapsedTime: {Main.timer1.Elapsed}");
            //ModelCore.UI.WriteLine($"Main.Run SoilLayer ElapsedTime: {Main.soilLayerTimer.Elapsed}");


            if (Timestep > 0)
                ClimateRegionData.SetAllEcoregionsFutureAnnualClimate(ModelCore.CurrentTime);

            if (ModelCore.CurrentTime % Timestep == 0)
            {
                // Write monthly log file:
                // Output must reflect the order of operation:
                int[] months = new int[12] { 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5 };

                if (OtherData.CalibrateMode)
                    months = new int[12] { 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5 };

                for (int i = 0; i < 12; i++)
                {
                    int month = months[i];
                    Outputs.WriteMonthlyLogFile(month);
                }
                Outputs.WritePrimaryLogFile(ModelCore.CurrentTime);
                Outputs.WriteShortPrimaryLogFile(ModelCore.CurrentTime);
                Outputs.WriteMaps();
                for (var i = 0; i < 12; ++i)
                    Outputs.WriteMonthlyMaps(months[i]);
                Outputs.WriteReproductionLog(PlugIn.ModelCore.CurrentTime);
                Establishment.LogEstablishment();
                if (PlugIn.InputCommunityMapNames != null && ModelCore.CurrentTime % PlugIn.InputCommunityMapFrequency == 0)
                    Outputs.WriteCommunityMaps();
            }

        }

        public void ExtensionBaseRun()
        {
            //bool isSuccessionTimestep = (Model.Core.CurrentTime % Timestep == 0);
            //IEnumerable<ActiveSite> sites;
            //if (isSuccessionTimestep)
            //    sites = Model.Core.Landscape.ActiveSites;
            //else
            //    sites = disturbedSites;

            var sites = ModelCore.Landscape.ActiveSites;

            var s = new Stopwatch();
            s.Restart();
            ExtensionBaseAgeCohorts(sites);
            ModelCore.UI.WriteLine($"ExtensionBase.Run.AgeCohorts time: {s.Elapsed}");
            s.Restart();
            ComputeShade(sites);
            ModelCore.UI.WriteLine($"ExtensionBase.Run.ComputeShade time: {s.Elapsed}");
            s.Restart();
            ReproduceCohorts(sites);
            ModelCore.UI.WriteLine($"ExtensionBase.Run.ReproduceCohorts time: {s.Elapsed}");

            //if (!isSuccessionTimestep)
            //    SiteVars.Disturbed.ActiveSiteValues = false;
        }

        public void ExtensionBaseAgeCohorts(IEnumerable<ActiveSite> sites)
        {
            //int? succTimestep = null;
            //if (isSuccessionTimestep)
            //{
            //    succTimestep = Timestep;
            //    ShowProgress = true;
            //}
            //else
            //{
            //    ShowProgress = false;
            //}

            ShowProgress = true;

            ProgressBar progressBar = null;
            if (ShowProgress)
            {
                System.Console.WriteLine("growing cohorts ...");
                prevSiteDataIndex = null;
                progressBar = ModelCore.UI.CreateProgressMeter(ModelCore.Landscape.ActiveSiteCount); // NewProgressBar();
            }

            var locker = new object();

            //AvailableN.CohortMineralNfraction = new Dictionary<ActiveSite, Dictionary<int, Dictionary<int, double>>>();
            //AvailableN.CohortMineralNallocation = new Dictionary<ActiveSite, Dictionary<int, Dictionary<int, double>>>();

            var parallel = true;
            if (parallel)
            {
                Parallel.ForEach(sites, site => 
                {
                    //ushort deltaTime = (ushort)(ModelCore.CurrentTime - SiteVars.TimeOfLast[site]);
                    AgeCohorts(site, 1, 1);
                    SiteVars.TimeOfLast[site] = ModelCore.CurrentTime;

                    if (ShowProgress)
                    {
                        lock (locker)
                            progressBar.IncrementWorkDone(1);
                    }
                });
            }
            else
            {
                foreach (ActiveSite site in sites)
                {
                    //ushort deltaTime = (ushort)(ModelCore.CurrentTime - SiteVars.TimeOfLast[site]);
                    AgeCohorts(site, 1, 1);
                    SiteVars.TimeOfLast[site] = ModelCore.CurrentTime;

                    if (ShowProgress)
                    {
                        lock (locker)
                            progressBar.IncrementWorkDone(1);
                    }
                }
            }

            //if (ShowProgress)
            //    CleanUp(progressBar);
        }

        //private void Update(ProgressBar progressBar,
        //            uint currentSiteDataIndex)
        //{
        //    uint increment = (uint)(prevSiteDataIndex.HasValue
        //                                ? (currentSiteDataIndex - prevSiteDataIndex.Value)
        //                                : currentSiteDataIndex);
        //    progressBar.IncrementWorkDone(increment);
        //    prevSiteDataIndex = currentSiteDataIndex;
        //}


        ////---------------------------------------------------------------------

        //private void CleanUp(ProgressBar progressBar)
        //{
        //    if (!prevSiteDataIndex.HasValue)
        //    {
        //        //    Then no sites were processed; the site iterator was a
        //        //    disturbed-sites iterator, and there were no disturbed
        //        //    sites.  So increment the progress bar to 100%.
        //        progressBar.IncrementWorkDone((uint)ModelCore.Landscape.ActiveSiteCount);
        //    }
        //}

        private void AssignTempHydroUnits()
        {
            ModelCore.UI.WriteLine("AssignTempHydroUnits: Start");

            // debug:
            if (TempHydroUnits == null) ModelCore.UI.WriteLine("TempHydroUnits is null");

            // disable all thus, so I only turn on those that are needed for the year
            foreach (var thu in TempHydroUnits)
            {
                thu.SiteCount = 0;
                thu.InUseForYear = false;
            }

            var nonForestCount = 0;

            var timeSinceLastBurn = new Dictionary<int, int>();

            SiteVars.ForestTypeName = ModelCore.GetSiteVar<string>("Output.ForestType");
            SiteVars.TimeOfLastBurn = ModelCore.GetSiteVar<int>("Fire.TimeOfLastEvent");
            SiteVars.Slope = ModelCore.GetSiteVar<ushort>("Fire.Slope");
            SiteVars.Aspect = ModelCore.GetSiteVar<ushort>("Fire.Aspect");

            // debug:
            if (SiteVars.ForestTypeName == null) ModelCore.UI.WriteLine("SiteVars.ForestTypeName is null");

            foreach (var site in ModelCore.Landscape.ActiveSites)
            {
                var climateRegion = ModelCore.Ecoregion[site];

                var reclassVegetation = SiteVars.ForestTypeName[site];
                if (reclassVegetation == "NonForest")
                {
                    ++nonForestCount;
                    continue;
                }

                var age = 0;
                if (SiteVars.TimeOfLastBurn != null && SiteVars.TimeOfLastBurn[site] > 0)
                {
                    age = ModelCore.CurrentTime - SiteVars.TimeOfLastBurn[site];
                    if (age < 0) age = 0;
                }
                else
                {
                    foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
                    {
                        foreach (ICohort cohort in speciesCohorts)
                        {
                            if (cohort.Data.Age > age) age = cohort.Data.Age;
                        }
                    }
                }

                if (!timeSinceLastBurn.TryGetValue(age, out var ageCount))
                    timeSinceLastBurn[age] = 0;

                ++timeSinceLastBurn[age];

                TempHydroUnit thu;

                var thuClimateRegionMatches = TempHydroUnits.Where(x => x.ClimateRegionName.Equals(climateRegion.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                if (!thuClimateRegionMatches.Any())
                    throw new ApplicationException($"No THUs match the ClimateRegion Name '{climateRegion.Name}' for site {site}");

                var thuReclassVegetationMatches = thuClimateRegionMatches.Where(x => x.ReclassVegetation.Equals(reclassVegetation, StringComparison.OrdinalIgnoreCase)).ToList();
                if (!thuReclassVegetationMatches.Any())
                    throw new ApplicationException($"No THUs for the ClimateRegion Name '{climateRegion.Name}' match the Reclass Vegetation '{reclassVegetation}' for site {site}");

                var thuAgeMatches = thuReclassVegetationMatches.Where(x => age >= x.MinAge && age <= x.MaxAge).ToList();
                if (!thuAgeMatches.Any())
                    throw new ApplicationException($"No THUs for the ClimateRegion Name '{climateRegion.Name}' and Reclass Vegetation '{reclassVegetation}' match the Age {age} for site {site}");

                if (SiteVars.Slope == null || SiteVars.Aspect == null)
                {
                    if (thuAgeMatches.Count != 1)
                        throw new ApplicationException($"More than one THU matches the ClimateRegion Name '{climateRegion.Name}', Reclass Vegetation '{reclassVegetation}', and Age {age} for site {site}");

                    thu = thuAgeMatches[0];
                }
                else
                {
                    var slope = (int)SiteVars.Slope[site];

                    var aspect = (int)SiteVars.Aspect[site];
                    ThuAspect thuAspect;
                    if (aspect <= 45 || aspect >= 315)
                        thuAspect = ThuAspect.North;
                    else if (aspect <= 135 || aspect >= 225)
                        thuAspect = ThuAspect.Other;
                    else
                        thuAspect = ThuAspect.South;

                    var thuSlopeMatches = thuAgeMatches.Where(x => slope >= x.MinSlope && slope <= x.MaxSlope).ToList();
                    if (!thuSlopeMatches.Any())
                        throw new ApplicationException($"No THUs for the ClimateRegion Name '{climateRegion.Name}', Reclass Vegetation '{reclassVegetation}' and Age {age} match the Slope {slope} for site {site}");

                    var thuAspectMatches = thuSlopeMatches.Where(x => x.Aspect == ThuAspect.All || x.Aspect == thuAspect).ToList();
                    if (!thuAspectMatches.Any())
                        throw new ApplicationException($"No THUs for the ClimateRegion Name '{climateRegion.Name}', Reclass Vegetation '{reclassVegetation}', Age {age}, and Slope {slope} match for the Aspect {aspect} ({thuAspect}) for site {site}");

                    if (thuAspectMatches.Count != 1)
                        throw new ApplicationException($"More than one THU matches the ClimateRegion Name '{climateRegion.Name}', Reclass Vegetation '{reclassVegetation}', Age {age}, Slope {slope}, and Aspect {aspect} ({thuAspect} or All) for site {site}");

                    thu = thuAspectMatches[0];
                }

                ++thu.SiteCount;

                thu.ClimateRegion = climateRegion;
                thu.InUseForYear = true;

                SiteVars.TempHydroUnit[site] = thu;
            }

            var thusInUse = TempHydroUnits.Where(x => x.InUseForYear).ToList();
            ModelCore.UI.WriteLine($"AssignTempHydroUnits: {thusInUse.Count} THUs in use:");
            foreach (var thu in thusInUse)
            {
                ModelCore.UI.WriteLine(thu.ToString());
            }

            ModelCore.UI.WriteLine("AssignTempHydroUnits: End");

        }

        private void RunTempHydroUnits()
        {
            // get annual daily weather for all active climate regions
            var calendarYear = Climate.FutureCalendarYear(ModelCore.CurrentTime);

            ClimateRegionData.SetAllEcoregionsFutureAnnualClimate(ModelCore.CurrentTime);

            ModelCore.UI.WriteLine("RunTempHydroUnits: Start");

            var parallel = true;
            if (parallel)
            {
                Parallel.ForEach(TempHydroUnits.Where(x => x.InUseForYear), thu => { thu.RunForYear(calendarYear); });
            }
            else
            {
                //TempHydroUnits = TempHydroUnits.Where(x => x.Name == "BurnedConifer_1_all_all").ToList();
                foreach (var thu in TempHydroUnits.Where(x => x.InUseForYear))
                {
                    ModelCore.UI.WriteLine($"Run THU '{thu}' for calendar year {calendarYear}");
                    thu.RunForYear(calendarYear);
                }
            }

            ModelCore.UI.WriteLine("RunTempHydroUnits: End");
        }

        //---------------------------------------------------------------------

        public override byte ComputeShade(ActiveSite site)
        {
            IEcoregion ecoregion = ModelCore.Ecoregion[site];

            byte finalShade = 0;

            if (!ecoregion.Active)
                return 0;

            for (byte shade = 5; shade >= 1; shade--)
            {
                if(ShadeLAI[shade] <=0 ) 
                {
                    string mesg = string.Format("Maximum LAI has not been defined for shade class {0}", shade);
                    throw new System.ApplicationException(mesg);
                }
                if (SiteVars.LAI[site] >= ShadeLAI[shade])
                {
                    finalShade = shade;
                    break;
                }
            }

            //PlugIn.ModelCore.UI.WriteLine("Yr={0},      Shade Calculation:  B_MAX={1}, B_ACT={2}, Shade={3}.", PlugIn.ModelCore.CurrentTime, B_MAX, B_ACT, finalShade);

            return finalShade;
        }
        //---------------------------------------------------------------------

        protected override void InitializeSite (ActiveSite site)
        {

            InitialBiomass initialBiomass = InitialBiomass.Compute(site, initialCommunity);
            SiteVars.MineralN[site] = Parameters.InitialMineralN;
            SiteVars.SoilPrimary[site].MicrobialCarbon = Parameters.InitialMicrobialC;
            SiteVars.SoilPrimary[site].MicrobialNitrogen = Parameters.InitialMicrobialN;
            SiteVars.SoilPrimary[site].EnzymaticConcentration = Parameters.InitialEnzymeConc;
            SiteVars.SoilPrimary[site].DOC = Parameters.InitialDOCFraction * SiteVars.SoilPrimary[site].Carbon;
            SiteVars.SoilPrimary[site].DON = Parameters.InitialDONFraction * SiteVars.SoilPrimary[site].Nitrogen;
        }


        public override void InitializeSites(string initialCommunitiesText, string initialCommunitiesMap, ICore modelCore)
        {
            ModelCore.UI.WriteLine("   Loading initial communities from file \"{0}\" ...", initialCommunitiesText);
            //Landis.Library.InitialCommunities.Universal.DatasetParser parser = new Landis.Library.InitialCommunities.Universal.DatasetParser(Timestep, ModelCore.Species, additionalCohortParameters);
            Landis.Library.InitialCommunities.Universal.DatasetParser parser = new Landis.Library.InitialCommunities.Universal.DatasetParser(Timestep, ModelCore.Species, additionalCohortParameters, initialCommunitiesText);
            Landis.Library.InitialCommunities.Universal.IDataset communities = Data.Load<Landis.Library.InitialCommunities.Universal.IDataset>(initialCommunitiesText, parser);

            ModelCore.UI.WriteLine("   Reading initial communities map \"{0}\" ...", initialCommunitiesMap);
            IInputRaster<UIntPixel> map;
            map = ModelCore.OpenRaster<UIntPixel>(initialCommunitiesMap);
            using (map)
            {
                UIntPixel pixel = map.BufferPixel;
                foreach (Site site in ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    uint mapCode = pixel.MapCode.Value;
                    if (!site.IsActive)
                        continue;

                    ActiveSite activeSite = (ActiveSite)site;
                    initialCommunity = communities.Find(mapCode);
                    if (initialCommunity == null)
                    {
                        //throw new ApplicationException(string.Format("Unknown map code for initial community: {0}", mapCode));
                        ModelCore.UI.WriteLine("   Map Code {0} has not initial community", mapCode);
                        SiteVars.Cohorts[site] = new SiteCohorts();
                        return;
                    }

                    InitializeSite(activeSite);
                }
            }
        }
        //---------------------------------------------------------------------
        // This method does not trigger reproduction
        public void CohortMortality(object sender, MortalityEventArgs eventArgs)
        {
            //PlugIn.ModelCore.UI.WriteLine("Cohort Partial Mortality:  {0}", eventArgs.Site);

            ExtensionType disturbanceType = eventArgs.DisturbanceType;
            ActiveSite site = eventArgs.Site;

            ICohort cohort = (ICohort)eventArgs.Cohort;

            double fractionPartialMortality = eventArgs.Reduction;
            double foliarInput = cohort.Data.AdditionalParameters.LeafBiomass * fractionPartialMortality;
            double woodInput = cohort.Data.AdditionalParameters.WoodBiomass * fractionPartialMortality;

            if (disturbanceType != null)
            {
                if (disturbanceType.IsMemberOf("disturbance:harvest"))
                {
                    SiteVars.HarvestPrescriptionName = ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");
                    if (!Disturbed[site]) // this is the first cohort killed/damaged
                    {
                        HarvestEffects.ReduceLayers(SiteVars.HarvestPrescriptionName[site], site);
                    }

                    double woodLoss = woodInput * (float)HarvestEffects.GetCohortWoodRemoval(site);
                    double foliarLoss = foliarInput * (float)HarvestEffects.GetCohortLeafRemoval(site);

                    if (eventArgs.Reduction >= 1)
                    {
                        SiteVars.SourceSink[site].Carbon += woodLoss * 0.47;
                        SiteVars.SourceSink[site].Carbon += foliarLoss * 0.47;
                    }

                    woodInput -= woodLoss;
                    foliarInput -= foliarLoss;
                }
                if (disturbanceType.IsMemberOf("disturbance:fire"))
                {
                    SiteVars.FireSeverity = ModelCore.GetSiteVar<byte>("Fire.Severity");

                    if (eventArgs.Reduction >= 1)
                    {
                        Landis.Library.Succession.Reproduction.CheckForPostFireRegen(eventArgs.Cohort, site);
                    }

                    if (!Disturbed[site]) // this is the first cohort killed/damaged
                    {
                        SiteVars.SmolderConsumption[site] = 0.0;
                        SiteVars.FlamingConsumption[site] = 0.0;
                        if (SiteVars.FireSeverity != null && SiteVars.FireSeverity[site] > 0)
                            FireEffects.ReduceLayers(SiteVars.FireSeverity[site], site);

                    }

                    if (eventArgs.Reduction >= 1)
                    {
                        double woodFireConsumption = woodInput * (float)FireEffects.ReductionsTable[(int)SiteVars.FireSeverity[site]].CoarseLitterReduction;
                        double foliarFireConsumption = foliarInput * (float)FireEffects.ReductionsTable[(int)SiteVars.FireSeverity[site]].FineLitterReduction;
                        SiteVars.SourceSink[site].Carbon += woodFireConsumption * 0.47;
                        SiteVars.SourceSink[site].Carbon += foliarFireConsumption * 0.47;

                        SiteVars.SmolderConsumption[site] += woodFireConsumption;
                        SiteVars.FlamingConsumption[site] += foliarFireConsumption;
                        woodInput -= (float)woodFireConsumption;
                        foliarInput -= (float)foliarFireConsumption;
                    }
                    else
                    {
                        double live_woodFireConsumption = woodInput * (float)FireEffects.ReductionsTable[(int)SiteVars.FireSeverity[site]].CohortWoodReduction;
                        double live_foliarFireConsumption = foliarInput * (float)FireEffects.ReductionsTable[(int)SiteVars.FireSeverity[site]].CohortLeafReduction;

                        SiteVars.SmolderConsumption[site] += live_woodFireConsumption;
                        SiteVars.FlamingConsumption[site] += live_foliarFireConsumption;
                        woodInput -= (float)live_woodFireConsumption;
                        foliarInput -= (float)live_foliarFireConsumption;
                    }
                }
                else
                {
                    // If not fire, check for resprouting:
                    Landis.Library.Succession.Reproduction.CheckForResprouting(eventArgs.Cohort, site);
                }
            }

            ForestFloor.AddWoodLitter(woodInput, cohort.Species, site);
            ForestFloor.AddFoliageLitter(foliarInput, cohort.Species, site);

            Roots.AddCoarseRootLitter(Roots.CalculateCoarseRoot(cohort, cohort.Data.AdditionalParameters.WoodBiomass * fractionPartialMortality), cohort, cohort.Species, site);
            Roots.AddFineRootLitter(Roots.CalculateFineRoot(cohort, cohort.Data.AdditionalParameters.LeafBiomass * fractionPartialMortality), cohort, cohort.Species, site);

            //PlugIn.ModelCore.UI.WriteLine("EVENT: Cohort Partial Mortality: species={0}, age={1}, disturbance={2}.", cohort.Species.Name, cohort.Age, disturbanceType);
            //PlugIn.ModelCore.UI.WriteLine("       Cohort Reductions:  Foliar={0:0.00}.  Wood={1:0.00}.", HarvestEffects.GetCohortLeafRemoval(site), HarvestEffects.GetCohortLeafRemoval(site));
            //PlugIn.ModelCore.UI.WriteLine("       InputB/TotalB:  Foliar={0:0.00}/{1:0.00}, Wood={2:0.0}/{3:0.0}.", foliarInput, cohort.LeafBiomass, woodInput, cohort.WoodBiomass);
            if (disturbanceType != null)
                Disturbed[site] = true;

            return;
        }

        //---------------------------------------------------------------------
        //Grows the cohorts for future climate
        protected override void AgeCohorts(ActiveSite site,
                                           ushort years,
                                           int?  successionTimestep)
        {
            Main.Run(site, years, successionTimestep.HasValue);

        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines if there is sufficient light at a site for a species to
        /// germinate/resprout.
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool SufficientLight(ISpecies species, ActiveSite site)
        {

            //PlugIn.ModelCore.UI.WriteLine("  Calculating Sufficient Light from Succession.");
            byte siteShade = ModelCore.GetSiteVar<byte>("Shade")[site];

            double lightProbability = 0.0;
            bool found = false;

            foreach (ISufficientLight lights in sufficientLight)
            {

                //PlugIn.ModelCore.UI.WriteLine("Sufficient Light:  ShadeClass={0}, Prob0={1}.", lights.ShadeClass, lights.ProbabilityLight0);
                if (lights.ShadeClass == SpeciesData.ShadeTolerance[species])
                {
                    if (siteShade == 0) lightProbability = lights.ProbabilityLight0;
                    if (siteShade == 1) lightProbability = lights.ProbabilityLight1;
                    if (siteShade == 2) lightProbability = lights.ProbabilityLight2;
                    if (siteShade == 3) lightProbability = lights.ProbabilityLight3;
                    if (siteShade == 4) lightProbability = lights.ProbabilityLight4;
                    if (siteShade == 5) lightProbability = lights.ProbabilityLight5;
                    found = true;
                }
            }

            if (!found)
                ModelCore.UI.WriteLine("A Sufficient Light value was not found for {0}.", species.Name);

            return modelCore.GenerateUniform() < lightProbability;

        }
        //---------------------------------------------------------------------
        
        /// <summary>
        /// Add a new cohort to a site following reproduction or planting.  Does not include initial communities.
        /// This is a Delegate method to base succession.
        /// </summary>

        public void AddNewCohort(ISpecies species, ActiveSite site, string reproductionType, double propBiomass = 1.0)
        {
            float[] initialBiomass = CohortBiomass.InitialBiomass(species, SiteVars.Cohorts[site], site);

            ExpandoObject woodLeafBiomasses = new ExpandoObject();
            dynamic tempObject = woodLeafBiomasses;
            tempObject.WoodBiomass = initialBiomass[0];
            tempObject.LeafBiomass = initialBiomass[1];

            SiteVars.Cohorts[site].AddNewCohort(species, 1, System.Convert.ToInt32(initialBiomass[0] + initialBiomass[1]), woodLeafBiomasses);

            if (reproductionType == "plant")
                SpeciesByPlant[species.Index]++;
            else if (reproductionType == "serotiny")
                SpeciesBySerotiny[species.Index]++;
            else if (reproductionType == "resprout")
                SpeciesByResprout[species.Index]++;
            else if (reproductionType == "seed")
                SpeciesBySeed[species.Index]++;

        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Determines if a species can establish on a site.
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool Establish(ISpecies species, ActiveSite site)
        {
            double establishProbability = Establishment.Calculate(species, site);
            return modelCore.GenerateUniform() < establishProbability;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if a species can establish on a site.
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool PlantingEstablish(ISpecies species, ActiveSite site)
        {
            IEcoregion ecoregion = modelCore.Ecoregion[site];
            double establishProbability = Establishment.Calculate(species, site); //, Timestep); // SpeciesData.EstablishProbability[species][ecoregion];

            return establishProbability > 0.0;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if there is a mature cohort at a site.
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool MaturePresent(ISpecies species, ActiveSite site)
        {
            return SiteVars.Cohorts[site].IsMaturePresent(species);
        }
        
        //---------------------------------------------------------------------
        // Outmoded but required?

        //public static void SiteDisturbed(object sender,
        //                                 Landis.Library.UniversalCohorts.DisturbanceEventArgs eventArgs)
        //{
        //    ModelCore.UI.WriteLine("  Calculating Fire or Harvest Effects.");

        //    ExtensionType disturbanceType = eventArgs.DisturbanceType;
        //    ActiveSite site = eventArgs.Site;

        //    if (disturbanceType.IsMemberOf("disturbance:fire"))
        //    {
        //        SiteVars.FireSeverity = ModelCore.GetSiteVar<byte>("Fire.Severity");
        //        if (SiteVars.FireSeverity != null && SiteVars.FireSeverity[site] > 0)
        //            FireEffects.ReduceLayers(SiteVars.FireSeverity[site], site);
        //    }
        //    if (disturbanceType.IsMemberOf("disturbance:harvest"))
        //    {
        //        HarvestEffects.ReduceLayers(SiteVars.HarvestPrescriptionName[site], site);
        //    }
        //}

        //---------------------------------------------------------------------

        public static float ReduceInput(float poolInput,
                                          Percentage reductionPercentage,
                                          ActiveSite site)
        {
            float reduction = (poolInput * (float)reductionPercentage);

            SiteVars.SourceSink[site].Carbon += (double)reduction * 0.47;

            return (poolInput - reduction);
        }

        private void InitializeTempHydroUnits()
        {
            string errorMessage;

            // read shaw gipl config file paths
            var inputFileParser = new SimpleFileParser(Parameters.ShawGiplConfigFile, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                throw new ApplicationException($"ShawGiplConfigFile : {errorMessage}");

            if (!inputFileParser.TryParse("ListThus", out string thuFilePath, out errorMessage))
                throw new ApplicationException($"ShawGiplConfigFile : {errorMessage}");

            if (!inputFileParser.TryParse("ShawGeneralInputs", out string shawInputFilePath, out errorMessage))
                throw new ApplicationException($"ShawGiplConfigFile : {errorMessage}");

            if (!inputFileParser.TryParse("ShawPlantTypes", out string plantFilePath, out errorMessage))
                throw new ApplicationException($"ShawGiplConfigFile : {errorMessage}");

            if (!inputFileParser.TryParse("ShawSoilTypes", out string soilFilePath, out errorMessage))
                throw new ApplicationException($"ShawGiplConfigFile : {errorMessage}");

            if (!inputFileParser.TryParse("GiplProperties", out string giplPropertiesFilePath, out errorMessage))
                throw new ApplicationException($"ShawGiplConfigFile : {errorMessage}");


            // read plant file data
            if (!ReadThuCsvInputFile(out var plantFileData1, plantFilePath, TempHydroUnit.PlantFileHeaders, out errorMessage))
                throw new ApplicationException($"Error reading THU plant file {plantFilePath} : '{errorMessage}'");

            // convert list of tuples into a dictionary
            var plantFileData = plantFileData1.ToDictionary(x => x.Item1, x => x.Item2);

            // read soil file data
            if (!ReadThuCsvInputFile(out var soilFileData1, soilFilePath, TempHydroUnit.SoilFileHeaders, out errorMessage))
                throw new ApplicationException($"Error reading THU soil file {soilFilePath} : '{errorMessage}'");

            // convert list of tuples into a dictionary
            var soilFileData = soilFileData1.ToDictionary(x => x.Item1, x => x.Item2);

            // read master thu file data
            if (!ReadThuCsvInputFile(out var thuFileData, thuFilePath, TempHydroUnit.ThuFileHeaders, out errorMessage))
                throw new ApplicationException($"Error reading master THU file {thuFilePath} : '{errorMessage}'");

            // global initialization for Shaw
            if (!ShawDamm.ShawDamm.GlobalInitialization(shawInputFilePath, out errorMessage))
                throw new ApplicationException($"Error with Shaw Global Initialization: '{errorMessage}'");

            // global initialization for Gipl
            if (!GiplDamm.GiplDamm.GlobalInitialization(giplPropertiesFilePath, out errorMessage))
                throw new ApplicationException($"Error with Gipl Global Initialization: '{errorMessage}'");


            // instantiate THUs
            TempHydroUnits = new List<TempHydroUnit>();
            foreach (var row in thuFileData)
            {
                TempHydroUnits.Add(new TempHydroUnit(row.Item1, row.Item2, plantFileData, soilFileData));
            }

            var thuNumbers = TempHydroUnits.Select(x => x.Number).ToList();
            var duplicateThuNumbers = thuNumbers.Where(x => TempHydroUnits.Count(y => y.Number == x) > 1).ToList();
            if (duplicateThuNumbers.Any())
                throw new ApplicationException($"Duplicate ThuNumbers found: '{string.Join(",", duplicateThuNumbers)}'");
        }

        /// <summary>Reads the CSV input file.
        /// Returns each row as a Tuple with Item1 as the input in the first column.  Each row's data is a dictionary of headers -&gt; values as Item2</summary>
        /// <param name="rowData">The row dictionary.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        private bool ReadThuCsvInputFile(out List<Tuple<string, Dictionary<string, string>>> rowData, string filePath, string[] headers, out string errorMessage)
        {
            errorMessage = string.Empty;
            rowData = new List<Tuple<string, Dictionary<string, string>>>();

            string[] fileData;
            try
            {
                fileData = File.ReadAllLines(filePath);
            }
            catch (Exception ex)
            {
                errorMessage = $"Exception : {ex.Message}";
                return false;
            }

            // split the rows and remove blank rows
            var data = fileData.Select(x => x.Split(',').Select(y => y.Trim()).ToList()).Where(x => x.Any(y => !string.IsNullOrEmpty(y))).ToList();

            // the first row should have the headers
            var headerIndices = headers.Select(x => data.First().IndexOf(x)).ToList();
            var missingHeaders = headerIndices.Select((x, i) => x < 0 ? headers[i] : string.Empty).Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (missingHeaders.Any())
            {
                errorMessage = $"missing header(s) : '{string.Join(",", missingHeaders)}'";
                return false;
            }

            var hasEnabledCol = data[0][0].Equals("Enabled", StringComparison.OrdinalIgnoreCase);
            var keyColumn = hasEnabledCol ? 1 : 0;
            var keyHeader = data[0][keyColumn];

            // make key-value dictionaries for each row, keyed by the (non-blank) value in the first column
            data.RemoveAt(0);
            foreach (var row in data)
            {
                // skip disabled rows
                if (hasEnabledCol && (!bool.TryParse(row[0], out var enabled) || !enabled))
                    continue;

                var key = row[keyColumn];

                if (string.IsNullOrEmpty(key))
                    continue;

                if (rowData.Find(x => x.Item1.Equals(key, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    errorMessage = $"Duplicate {keyHeader} '{key}'";
                    return false;
                }

                var rowDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                for (var i = 0; i < headers.Length; ++i)
                    rowDict[headers[i]] = headerIndices[i] < row.Count ? row[headerIndices[i]] : string.Empty;

                rowData.Add(new Tuple<string, Dictionary<string, string>>(key, rowDict));
            }

            return true;
        }
    }
}
