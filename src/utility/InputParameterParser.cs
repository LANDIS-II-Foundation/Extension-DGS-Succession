//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.Utilities;
using Landis.Library.Succession;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using Landis.Library.InitialCommunities.Universal;

namespace Landis.Extension.Succession.DGS
{
    /// <summary>
    /// A parser that reads biomass succession parameters from text input.
    /// </summary>
    public class InputParametersParser
        : TextParser<IInputParameters>
    {
        public override string LandisDataValue
        {
            get
            {
                return PlugIn.ExtensionName;
            }
        }


        public static class Names
        {
            public const string SpeciesParameters = "SpeciesParameters";
            public const string FunctionalGroupParameters = "FunctionalGroupParameters";
            public const string FireReductionParameters = "FireReductionParameters";
            public const string HarvestReductionParameters = "HarvestReductionParameters";
        }

        //---------------------------------------------------------------------

        private IEcoregionDataset ecoregionDataset;
        private ISpeciesDataset speciesDataset;
        private Dictionary<string, int> speciesLineNums;
        private InputVar<string> speciesName;

        //---------------------------------------------------------------------

        static InputParametersParser()
        {
            SeedingAlgorithmsUtil.RegisterForInputValues();
            RegisterForInputValues();
            Percentage dummy = new Percentage();

        }

        //---------------------------------------------------------------------

        public InputParametersParser()
        {
            this.ecoregionDataset = PlugIn.ModelCore.Ecoregions;
            this.speciesDataset = PlugIn.ModelCore.Species;
            this.speciesLineNums = new Dictionary<string, int>();
            this.speciesName = new InputVar<string>("Species");

        }

        //---------------------------------------------------------------------

        protected override IInputParameters Parse()
        {

            ReadLandisDataVar();

            int numLitterTypes = 4;
            int numFunctionalTypes = 25;

            InputParameters parameters = new InputParameters(speciesDataset, numLitterTypes, numFunctionalTypes);

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            InputVar<bool> calimode = new InputVar<bool>("CalibrateMode");
            if (ReadOptionalVar(calimode))
                parameters.CalibrateMode = calimode.Value;
            else
                parameters.CalibrateMode = false;

            InputVar<string> climateConfigFile = new InputVar<string>("ClimateConfigFile");
            ReadVar(climateConfigFile);
            parameters.ClimateConfigFile = climateConfigFile.Value;

            InputVar<double> ans = new InputVar<double>("AtmosphericNSlope");
            ReadVar(ans);
            parameters.SetAtmosNslope(ans.Value);

            InputVar<double> ani = new InputVar<double>("AtmosphericNIntercept");
            ReadVar(ani);
            parameters.SetAtmosNintercept(ani.Value);


            //----------------Landscape Maps-----------------------------------------------------------------
            
            InputVar<string> initCommunities = new InputVar<string>("InitialCommunities");
            ReadVar(initCommunities);
            parameters.InitialCommunities = initCommunities.Value;

            InputVar<string> communitiesMap = new InputVar<string>("InitialCommunitiesMap");
            ReadVar(communitiesMap);
            parameters.InitialCommunitiesMap = communitiesMap.Value;

            InputVar<double> lat = new InputVar<double>("Latitude");
            ReadVar(lat);
            parameters.SetLatitude(lat.Value);

            //-----------------Shaw Gipl----------------------------------------------------------------

            InputVar<string> shawGiplConfigFile = new InputVar<string>("ShawGiplConfigFile");
            ReadVar(shawGiplConfigFile);
            parameters.ShawGiplConfigFile = shawGiplConfigFile.Value;

            //-----------------Soil Maps----------------------------------------------------------------

            InputVar<string> soilDepthMapName = new InputVar<string>("SoilDepthMapName");
            ReadVar(soilDepthMapName);
            parameters.SoilDepthMapName = soilDepthMapName.Value;

            InputVar<string> soilDrainMapName = new InputVar<string>("SoilDrainMapName");
            ReadVar(soilDrainMapName);
            parameters.SoilDrainMapName = soilDrainMapName.Value;

            InputVar<string> soilBaseFlowMapName = new InputVar<string>("SoilBaseFlowMapName");
            ReadVar(soilBaseFlowMapName);
            parameters.SoilBaseFlowMapName = soilBaseFlowMapName.Value;

            InputVar<string> soilStormFlowMapName = new InputVar<string>("SoilStormFlowMapName");
            ReadVar(soilStormFlowMapName);
            parameters.SoilStormFlowMapName = soilStormFlowMapName.Value;

            InputVar<string> soilFCMapName = new InputVar<string>("SoilFieldCapacityMapName");
            ReadVar(soilFCMapName);
            parameters.SoilFieldCapacityMapName = soilFCMapName.Value;

            InputVar<string> soilWPMapName = new InputVar<string>("SoilWiltingPointMapName");
            ReadVar(soilWPMapName);
            parameters.SoilWiltingPointMapName = soilWPMapName.Value;

            InputVar<string> soilSandMapName = new InputVar<string>("SoilPercentSandMapName");
            ReadVar(soilSandMapName);
            parameters.SoilPercentSandMapName = soilSandMapName.Value;

            InputVar<string> soilClayMapName = new InputVar<string>("SoilPercentClayMapName");
            ReadVar(soilClayMapName);
            parameters.SoilPercentClayMapName = soilClayMapName.Value;

            InputVar<string> soilBDMapName = new InputVar<string>("SoilBulkDensityMapName");
            ReadVar(soilBDMapName);
            parameters.SoilBulkDensityMapName = soilBDMapName.Value;

            InputVar<string> soilPDMapName = new InputVar<string>("SoilParticleDensityMapName");
            ReadVar(soilPDMapName);
            parameters.SoilParticleDensityMapName = soilPDMapName.Value;

            InputVar<string> socMapName = new InputVar<string>("InitialSOC_PrimaryMapName");
            ReadVar(socMapName);
            parameters.InitialSOC_PrimaryMapName = socMapName.Value;

            InputVar<string> sonMapName = new InputVar<string>("InitialSON_PrimaryMapName");
            ReadVar(sonMapName);
            parameters.InitialSON_PrimaryMapName = sonMapName.Value;

            InputVar<string> deadSurfMapName = new InputVar<string>("InitialDeadWoodSurfaceMapName");
            ReadVar(deadSurfMapName);
            parameters.InitialDeadSurfaceMapName = deadSurfMapName.Value;

            InputVar<string> deadSoilMapName = new InputVar<string>("InitialDeadCoarseRootsMapName");
            ReadVar(deadSoilMapName);
            parameters.InitialDeadSoilMapName = deadSoilMapName.Value;

            //-----------------Soil Parameters----------------------------------------------------------------

            InputVar<double> iFF = new InputVar<double>("InitialFineFuels");
            ReadVar(iFF);
            parameters.SetInitFineFuels(iFF.Value);

            InputVar<double> iMN = new InputVar<double>("InitialMineralN");
            ReadVar(iMN);
            parameters.SetInitMineralN(iMN.Value);

            InputVar<double> denits = new InputVar<double>("DenitrificationRate");
            ReadVar(denits);
            parameters.SetDenitrif(denits.Value);

            InputVar<string> wt = new InputVar<string>("WaterDecayFunction");
            ReadVar(wt);
            parameters.WType = WParse(wt.Value);

            InputVar<double> iMicC = new InputVar<double>("InitialMicrobialC");
            ReadVar(iMicC);
            parameters.SetInitMicrobialC(iMicC.Value);

            InputVar<double> iMicN = new InputVar<double>("InitialMicrobialN");   
            ReadVar(iMicN);
            parameters.SetInitMicrobialN(iMicN.Value);

            InputVar<double> iEnzConc = new InputVar<double>("InitialEnzymeConc");
            ReadVar(iEnzConc);
            parameters.SetInitEnzymeConc(iEnzConc.Value);

            InputVar<double> aeSOMD = new InputVar<double>("ActEnergySOMDepoly");
            ReadVar(aeSOMD);
            parameters.SetActEnergySOMDepoly(aeSOMD.Value);

            InputVar<double> aeDOCU = new InputVar<double>("ActEnergyDOCUptake");
            ReadVar(aeDOCU);
            parameters.SetActEnergyDOCUptake(aeDOCU.Value);

            InputVar<double> econstSOMD = new InputVar<double>("ExpConstSOMDepoly");
            ReadVar(econstSOMD);
            parameters.SetECSOMDepoly(econstSOMD.Value);

            InputVar<double> econstDOCU = new InputVar<double>("ExpConstDOCUptake");
            ReadVar(econstDOCU);
            parameters.SetExpConstDOCUptake(econstDOCU.Value);

            InputVar<double> fracUSOM = new InputVar<double>("FractionSOMUnprotect");
            ReadVar(fracUSOM);
            parameters.SetFractionSOMUnprotect(fracUSOM.Value);

            InputVar<double> CNEnz = new InputVar<double>("CNEnzymes");
            ReadVar(CNEnz);
            parameters.SetCNEnzymes(CNEnz.Value);

            InputVar<double> kmSOMD = new InputVar<double>("KmSOMDepoly");
            ReadVar(kmSOMD);
            parameters.SetKmSOMDepoly(kmSOMD.Value);

            InputVar<double> KmDOCU = new InputVar<double>("KmDOCUptake");
            ReadVar(KmDOCU);
            parameters.SetKmDOCUptake(KmDOCU.Value);

            InputVar<double> EnvTurnR = new InputVar<double>("EnzTurnRate");
            ReadVar(EnvTurnR);
            parameters.SetEnzymeTurnRate(EnvTurnR.Value);

            InputVar<double> MTurnR = new InputVar<double>("MicrobialTurnRate");
            ReadVar(MTurnR);
            parameters.SetMicrobialTurnRate(MTurnR.Value);

            InputVar<double> CUE = new InputVar<double>("CarbonUseEfficiency");
            ReadVar(CUE);
            parameters.SetCarbonUseEfficiency(CUE.Value);

            InputVar<double> PEnzSOM = new InputVar<double>("PropEnzymeSOM");
            ReadVar(PEnzSOM);
            parameters.SetPropEnzymeSOM(PEnzSOM.Value);

            InputVar<double> PEnzCProd = new InputVar<double>("PropCEnzymeProduction");
            ReadVar(PEnzCProd);
            parameters.SetPropCEnzymeProduction(PEnzCProd.Value);

            InputVar<double> PEnzNProd = new InputVar<double>("PropNEnzymeProduction");
            ReadVar(PEnzNProd);
            parameters.SetPropNEnzymeProduction(PEnzNProd.Value);

            InputVar<double> fracDeadMB = new InputVar<double>("FractDeadMicrobialBiomassSOM");
            ReadVar(fracDeadMB);
            parameters.SetFractDeadMicrobialBiomassSOM(fracDeadMB.Value)
                ;
            InputVar<double> MMO2 = new InputVar<double>("MMConstantO2");
            ReadVar(MMO2);
            parameters.SetMMConstantO2(MMO2.Value);

            InputVar<double> DiffO2 = new InputVar<double>("DiffConstantO2");
            ReadVar(DiffO2);
            parameters.SetDiffConstantO2(DiffO2.Value);

            InputVar<double> DiffSOMLiq = new InputVar<double>("DiffConstantSOMLiquid");
            ReadVar(DiffSOMLiq);
            parameters.SetDiffConstantSOMLiquid(DiffSOMLiq.Value);

            InputVar<double> FracVolO2 = new InputVar<double>("FractionVolumeO2");
            ReadVar(FracVolO2);
            parameters.SetFractionVolumeO2(FracVolO2.Value);     

            InputVar<double> iDOCf = new InputVar<double>("DOCFraction");
            ReadVar(iDOCf);
            parameters.SetInitDOCFraction(iDOCf.Value);

            InputVar<double> iDONf = new InputVar<double>("DONFraction");
            ReadVar(iDONf);
            parameters.SetInitDONFraction(iDONf.Value);

            InputVar<double> fracDOC = new InputVar<double>("FractionLitterToDOC");
            ReadVar(fracDOC);
            parameters.SetFractionDOC(fracDOC.Value);

            //InputVar<double> sma = new InputVar<double>("SoilMoistureA");
            //ReadVar(sma);
            //parameters.SetSoilMoistureA(sma.Value);

            //InputVar<double> smb = new InputVar<double>("SoilMoistureB");
            //ReadVar(smb);
            //parameters.SetSoilMoistureB(smb.Value);

            

            //-----------------Dispersal Parameters----------------------------------------------------------------

            InputVar<SeedingAlgorithms> seedAlg = new InputVar<SeedingAlgorithms>("SeedingAlgorithm");
            ReadVar(seedAlg);
            parameters.SeedAlgorithm = seedAlg.Value;

            InputVar<double> pea = new InputVar<double>("ProbabilityEstablishAdjust");
            ReadVar(pea);
            parameters.ProbEstablishAdjustment = pea.Value;


            //InputVar<double> drsoms = new InputVar<double>("DecayRateSurf");
            //ReadVar(drsoms);
            //parameters.SetDecayRateSurf(drsoms.Value);

            //InputVar<double> drsom1 = new InputVar<double>("DecayRateSOM1");
            //ReadVar(drsom1);
            //parameters.SetDecayRateSOM1(drsom1.Value);

            //InputVar<double> drsom2 = new InputVar<double>("DecayRateSOM2");
            //ReadVar(drsom2);
            //parameters.SetDecayRateSOM2(drsom2.Value);

            //InputVar<double> drsom3 = new InputVar<double>("DecayRateSOM3");
            //ReadVar(drsom3);
            //parameters.SetDecayRateSOM3(drsom3.Value);

            //--------------------------
            //  LAI and light table

            //ReadName("MaximumLAI"); //"AvailableLightBiomass");
            //InputVar<byte> shadeClassVar = new InputVar<byte>("Shade Class");
            //InputVar<double> maxLAI = new InputVar<double>("Maximum LAI");

            //for (byte shadeClass = 1; shadeClass <= 5; shadeClass++)
            //{
            //    if (AtEndOfInput)
            //        throw NewParseException("Expected a line with available light class {0}", shadeClass);

            //    StringReader currentLine = new StringReader(CurrentLine);
            //    ReadValue(shadeClassVar, currentLine);
            //    if (shadeClassVar.Value.Actual != shadeClass)
            //        throw new InputValueException(shadeClassVar.Value.String,
            //                                      "Expected the available light class {0}", shadeClass);

            //    ReadValue(maxLAI, currentLine);
            //    parameters.SetMaximumShadeLAI(shadeClass, maxLAI.Value);

            //    CheckNoDataAfter("the " + maxLAI + " column", currentLine);
            //    GetNextLine();
            //}

            //----------------------------------------------------------
            //  Read table of sufficient light probabilities.
            //  Available light classes are in increasing order.
            //ReadName("LightEstablishmentTable");

            //InputVar<byte> sc = new InputVar<byte>("Available Light Class");
            //InputVar<double> pl0 = new InputVar<double>("Probability of Germination - Light Level 0");
            //InputVar<double> pl1 = new InputVar<double>("Probability of Germination - Light Level 1");
            //InputVar<double> pl2 = new InputVar<double>("Probability of Germination - Light Level 2");
            //InputVar<double> pl3 = new InputVar<double>("Probability of Germination - Light Level 3");
            //InputVar<double> pl4 = new InputVar<double>("Probability of Germination - Light Level 4");
            //InputVar<double> pl5 = new InputVar<double>("Probability of Germination - Light Level 5");

            //int previousNumber = 0;

            //while (! AtEndOfInput && CurrentName != Names.SpeciesParameters
            //                      && previousNumber != 6) {
            //    StringReader currentLine = new StringReader(CurrentLine);

            //    ISufficientLight suffLight = new SufficientLight();

            //    ReadValue(sc, currentLine);
            //    suffLight.ShadeClass = sc.Value;

            //    //  Check that the current shade class is 1 more than
            //    //  the previous number (numbers are must be in increasing order).
            //    if (sc.Value.Actual != (byte) previousNumber + 1)
            //        throw new InputValueException(sc.Value.String,
            //                                      "Expected the severity number {0}",
            //                                      previousNumber + 1);
            //    previousNumber = (int) sc.Value.Actual;

            //    ReadValue(pl0, currentLine);
            //    suffLight.ProbabilityLight0 = pl0.Value;

            //    ReadValue(pl1, currentLine);
            //    suffLight.ProbabilityLight1 = pl1.Value;

            //    ReadValue(pl2, currentLine);
            //    suffLight.ProbabilityLight2 = pl2.Value;

            //    ReadValue(pl3, currentLine);
            //    suffLight.ProbabilityLight3 = pl3.Value;

            //    ReadValue(pl4, currentLine);
            //    suffLight.ProbabilityLight4 = pl4.Value;

            //    ReadValue(pl5, currentLine);
            //    suffLight.ProbabilityLight5 = pl5.Value;

            //    parameters.LightClassProbabilities.Add(suffLight);

            //    CheckNoDataAfter("the " + pl5.Name + " column",
            //                     currentLine);
            //    GetNextLine();
            //}
            //if (parameters.LightClassProbabilities.Count == 0)
            //    throw NewParseException("No sufficient light probabilities defined.");
            //if (previousNumber != 5)
            //    throw NewParseException("Expected shade class {0}", previousNumber + 1);

            //-------------------------
            //  Species Parameters table

            var csv = new InputVar<string>("SpeciesParameters");
            ReadVar(csv);
            var speciesParser = new CSVParser();
            var speciesTable = speciesParser.ParseToDataTable(csv.Value);

            foreach (DataRow row in speciesTable.Rows)
            {
                var species = ReadSpecies(System.Convert.ToString(row["SpeciesCode"]));
                
                parameters.SetGDDmin(species, System.Convert.ToInt32(row["GDDMinimum"]));
                parameters.SetGDDmax(species, System.Convert.ToInt32(row["GDDMaximum"]));
                parameters.SetMinJanTemp(species, System.Convert.ToInt32(row["MinJanuaryT"]));
                parameters.SetMaxDrought(species, System.Convert.ToDouble(row["MaxDrought"]));
                parameters.SetLightLAIShape(species, System.Convert.ToDouble(row["LightLAIShape"]));
                parameters.SetLightLAIScale(species, System.Convert.ToDouble(row["LightLAIScale"]));
                parameters.SetLightLAILocation(species, System.Convert.ToDouble(row["LightLAILocation"]));
                parameters.SetLightLAIAdjust(species, ReadLightLAIAdjust(row));

                parameters.AdventRoots[species] = System.Convert.ToBoolean(row["AdvRoots"]);
                parameters.NFixer[species] = System.Convert.ToBoolean(row["NitrogenFixer"]);
                parameters.Epicormic[species] = System.Convert.ToBoolean(row["Epicormic"]);
                
                parameters.SetKLAI(species, System.Convert.ToDouble(row["KLAI"]));
                parameters.SetMaxLAI(species, System.Convert.ToDouble(row["MaximumLAI"]));
                parameters.SetCompLimit(species, System.Convert.ToDouble(row["CompLimit"]));
                parameters.SetLeafBiomassToLAI(species, System.Convert.ToDouble(row["LeafBiomassToLAI"]));
                parameters.SetLeafLongevity(species, System.Convert.ToDouble(row["LeafLongevity"]));
                parameters.SetFoliageDropMonth(species, System.Convert.ToInt32(row["FoliageDropMonth"]));
                parameters.SetMaxANPP(species, System.Convert.ToInt32(row["MaximumANPP"]));
                parameters.SetMaxBiomass(species, System.Convert.ToInt32(row["MaximumBiomass"]));

                parameters.SetTemperatureCurve1(species, System.Convert.ToDouble(row["TemperatureCurve1"]));
                parameters.SetTemperatureCurve2(species, System.Convert.ToDouble(row["TemperatureCurve2"]));
                parameters.SetTemperatureCurve3(species, System.Convert.ToDouble(row["TemperatureCurve3"]));
                parameters.SetTemperatureCurve4(species, System.Convert.ToDouble(row["TemperatureCurve4"]));
                parameters.MoistureCurve1[species] = System.Convert.ToDouble(row["MoistureCurve1"]);
                parameters.MoistureCurve2[species] = System.Convert.ToDouble(row["MoistureCurve2"]);
                parameters.MoistureCurve3[species] = System.Convert.ToDouble(row["MoistureCurve3"]);
                parameters.MoistureCurve4[species] = System.Convert.ToDouble(row["MoistureCurve4"]);

                parameters.SetFractionANPPtoLeaf(species, System.Convert.ToDouble(row["FractionANPPtoLeaf"]));
                parameters.SetCoarseRootFraction(species, System.Convert.ToDouble(row["CoarseRootFraction"]));
                parameters.SetFineRootFraction(species, System.Convert.ToDouble(row["FineRootFraction"]));
                parameters.SetRootingDepth(species, System.Convert.ToInt32(row["RootDepth"]));
                parameters.SetMortalityShapeCurve(species, System.Convert.ToDouble(row["LongevityMortalityShape"]));

                parameters.SetLeafLignin(species, System.Convert.ToDouble(row["LeafLignin"]));
                parameters.SetFineRootLignin(species, System.Convert.ToDouble(row["FineRootLignin"]));
                parameters.SetWoodLignin(species, System.Convert.ToDouble(row["WoodLignin"]));
                parameters.SetCoarseRootLignin(species, System.Convert.ToDouble(row["CoarseRootLignin"]));
                parameters.SetLeafCN(species, System.Convert.ToDouble(row["LeafCN"]));
                parameters.SetFineRootCN(species, System.Convert.ToDouble(row["FineRootCN"]));
                parameters.SetWoodCN(species, System.Convert.ToDouble(row["WoodCN"]));
                parameters.SetCoarseRootCN(species, System.Convert.ToDouble(row["CoarseRootCN"]));
                parameters.SetFoliageLitterCN(species, System.Convert.ToDouble(row["FoliageLitterCN"]));

                parameters.SetWoodDecayRate(species, System.Convert.ToDouble(row["WoodDecayRate"]));
                parameters.SetMonthlyWoodMortality(species, System.Convert.ToDouble(row["MonthlyWoodMortality"]));
            }



            //ReadName("SpeciesParameters");

            //speciesLineNums.Clear();  //  If parser re-used (i.e., for testing purposes)

            //InputVar<int> ft = new InputVar<int>("Functional Type");
            //InputVar<bool> nt = new InputVar<bool>("Nitrogen Fixer");
            //InputVar<bool> adventRoots = new InputVar<bool>("Adventitous Roots");
            //InputVar<int> gddmn = new InputVar<int>("Growing Degree Day Minimum");
            //InputVar<int> gddmx = new InputVar<int>("Growing Degree Day Maximum");
            //InputVar<int> mjt = new InputVar<int>("Minimum January Temperature");
            //InputVar<double> maxd = new InputVar<double>("Maximum Allowable Drought");
            //InputVar<double> leafLongevity = new InputVar<double>("Leaf Longevity");
            //InputVar<bool> epicorm = new InputVar<bool>("Epicormic:  Y/N");
            //InputVar<double> leafLignin = new InputVar<double>("Leaf Percent Lignin");
            //InputVar<double> wLignin = new InputVar<double>("Wood Percent Lignin");
            //InputVar<double> crLignin = new InputVar<double>("Coarse Root Percent Lignin");
            //InputVar<double> frLignin = new InputVar<double>("Fine Root Percent Lignin");
            //InputVar<double> leafCN = new InputVar<double>("Leaf CN Ratio");
            //InputVar<double> woodCN = new InputVar<double>("Wood CN Ratio");
            //InputVar<double> cRootCN = new InputVar<double>("Coarse Root CN Ratio");
            //InputVar<double> foliarCN = new InputVar<double>("Foliage CN Ratio");
            //InputVar<double> fRootCN = new InputVar<double>("Fine Root CN Ratio");
            //InputVar<int> rootingDepth = new InputVar<int>("Rooting depth");
            //InputVar<int> maxANPP = new InputVar<int>("Maximum ANPP");
            //InputVar<int> maxBiomass = new InputVar<int>("Maximum Aboveground Biomass");
            //InputVar<int> fireTolerance = new InputVar<int>("Fire Tolerance");
            //InputVar<int> shadeTolerance = new InputVar<int>("Shade Tolerance");
            //InputVar<double> lightLaiShape = new InputVar<double>("LightLAIShape");
            //InputVar<double> lightLaiScale = new InputVar<double>("LightLAIScale");
            //InputVar<double> lightLaiLocation = new InputVar<double>("LightLAILocation");
            //InputVar<double> lightLaiAdjust = new InputVar<double>("LightLAIAdjust");
            //string lastColumn = "the " + lightLaiAdjust.Name + " column";

            //while (! AtEndOfInput && CurrentName != Names.FunctionalGroupParameters) {
            //    StringReader currentLine = new StringReader(CurrentLine);
            //    ISpecies species = ReadSpecies(currentLine);

                //ReadValue(ft, currentLine);
                //parameters.SetFunctionalType(species, ft.Value);

                //ReadValue(nt, currentLine);
                //parameters.NFixer[species] = nt.Value;

                //ReadValue(adventRoots, currentLine);
                //parameters.AdventRoots[species] = adventRoots.Value;

                //ReadValue(gddmn, currentLine);
                //parameters.SetGDDmin(species, gddmn.Value);

                //ReadValue(gddmx, currentLine);
                //parameters.SetGDDmax(species, gddmx.Value);

                //ReadValue(mjt, currentLine);
                //parameters.SetMinJanTemp(species, mjt.Value);

                //ReadValue(maxd, currentLine);
                //parameters.SetMaxDrought(species, maxd.Value);

                //ReadValue(leafLongevity, currentLine);
                //parameters.SetLeafLongevity(species, leafLongevity.Value);

                //ReadValue(epicorm, currentLine);
                //parameters.Epicormic[species] = epicorm.Value;

                //ReadValue(leafLignin, currentLine);
                //parameters.SetLeafLignin(species, leafLignin.Value);

                //ReadValue(frLignin, currentLine);
                //parameters.SetFineRootLignin(species, frLignin.Value);

                //ReadValue(wLignin, currentLine);
                //parameters.SetWoodLignin(species, wLignin.Value);

                //ReadValue(crLignin, currentLine);
                //parameters.SetCoarseRootLignin(species, crLignin.Value);

                //ReadValue(leafCN, currentLine);
                //parameters.SetLeafCN(species, leafCN.Value);

                //ReadValue(fRootCN, currentLine);
                //parameters.SetFineRootCN(species, fRootCN.Value);

                //ReadValue(woodCN, currentLine);
                //parameters.SetWoodCN(species, woodCN.Value);

                //ReadValue(cRootCN, currentLine);
                //parameters.SetCoarseRootCN(species, cRootCN.Value);

                //ReadValue(foliarCN, currentLine);
                //parameters.SetFoliageLitterCN(species, foliarCN.Value);

                //ReadValue(rootingDepth, currentLine);
                //parameters.SetRootingDepth(species, rootingDepth.Value);

                //ReadValue(maxANPP, currentLine);
                //parameters.SetMaxANPP(species, maxANPP.Value);

                //ReadValue(maxBiomass, currentLine);
                //parameters.SetMaxBiomass(species, maxBiomass.Value);

                //ReadValue(fireTolerance, currentLine);
                //parameters.SetFireTolerance(species, fireTolerance.Value);

                //ReadValue(shadeTolerance, currentLine);
                //parameters.SetShadeTolerance(species, shadeTolerance.Value);

                //ReadValue(lightLaiShape, currentLine);
                //parameters.SetLightLAIShape(species, lightLaiShape.Value);

                //ReadValue(lightLaiScale, currentLine);
                //parameters.SetLightLAIScale(species, lightLaiScale.Value);

                //ReadValue(lightLaiLocation, currentLine);
                //parameters.SetLightLAILocation(species, lightLaiLocation.Value);

                //ReadValue(lightLaiAdjust, currentLine);
                //parameters.SetLightLAIAdjust(species, lightLaiAdjust.Value);

            //    CheckNoDataAfter(lastColumn, currentLine);
            //    GetNextLine();
            //}

            ////--------- Read In Functional Group Table -------------------------------
            //PlugIn.ModelCore.UI.WriteLine("   Begin parsing FUNCTIONAL GROUP table.");

            //ReadName("FunctionalGroupParameters");
            ////string InitialEcoregionParameters = "InitialEcoregionParameters";

            //InputVar<string> ftname = new InputVar<string>("Name");
            //InputVar<int> ftindex = new InputVar<int>("Index (< 25)");
            //InputVar<double> tempcurve1 = new InputVar<double>("TempCurve(1)");
            //InputVar<double> tempcurve2 = new InputVar<double>("TempCurve(2)");
            //InputVar<double> tempcurve3 = new InputVar<double>("TempCurve(3)");
            //InputVar<double> tempcurve4 = new InputVar<double>("TempCurve(4)");
            //InputVar<double> fanppleaf = new InputVar<double>("FCFRAC: Leaf");
            //InputVar<double> btolai = new InputVar<double>("BTOLAI");
            //InputVar<double> klai = new InputVar<double>("KLAI");
            //InputVar<double> k = new InputVar<double>("K");
            //InputVar<double> maxlai = new InputVar<double>("MAXLAI");
            //InputVar<double> mwm = new InputVar<double>("Monthly Wood Mortality");
            //InputVar<double> wdr = new InputVar<double>("Wood Decay Rate");
            //InputVar<double> mortCurveShapeParm = new InputVar<double>("Mortality Curve Shape Parameter");
            //InputVar<int> foliageDropMonth = new InputVar<int>("Leaf or Needle Drop Month");

            //InputVar<double> moisturecurve1 = new InputVar<double>("MoistureCurve1");
            //InputVar<double> moisturecurve2 = new InputVar<double>("MoistureCurve2");
            //InputVar<double> moisturecurve3 = new InputVar<double>("MoistureCurve3");
            //InputVar<double> moisturecurve4 = new InputVar<double>("MoistureCurve4");
            //InputVar<double> coarseRootFraction = new InputVar<double>("CRootFrac");
            //InputVar<double> fineRootFraction = new InputVar<double>("FRootFrac");

            //while (! AtEndOfInput && CurrentName != Names.FireReductionParameters) {
            //    StringReader currentLine = new StringReader(CurrentLine);

            //    ReadValue(ftname , currentLine);

            //    ReadValue(ftindex , currentLine);
            //    int ln = (int) ftindex.Value.Actual;

            //    if(ln >= numFunctionalTypes)
            //        throw new InputValueException(ftindex.Value.String,
            //                                  "The index:  {0} exceeds the allowable number of functional groups, {1}",
            //                                  ftindex.Value.String, numFunctionalTypes-1);


            //    FunctionalType funcTParms = new FunctionalType();
            //    parameters.FunctionalTypes[ln] = funcTParms;

            //    ReadValue(tempcurve1, currentLine);
            //    funcTParms.TempCurve1 = tempcurve1.Value;

            //    ReadValue(tempcurve2, currentLine);
            //    funcTParms.TempCurve2 = tempcurve2.Value;

            //    ReadValue(tempcurve3, currentLine);
            //    funcTParms.TempCurve3 = tempcurve3.Value;

            //    ReadValue(tempcurve4, currentLine);
            //    funcTParms.TempCurve4 = tempcurve4.Value;

            //    ReadValue(fanppleaf, currentLine);
            //    funcTParms.FractionANPPtoLeaf = fanppleaf.Value;

            //    ReadValue(btolai, currentLine);
            //    funcTParms.BiomassToLAI = btolai.Value;

            //    ReadValue(klai, currentLine);
            //    funcTParms.KLAI = klai.Value;

            //    ReadValue(k, currentLine);
            //    funcTParms.K = k.Value;

            //    ReadValue(maxlai, currentLine);
            //    funcTParms.MaxLAI = maxlai.Value;

            //    ReadValue(moisturecurve1, currentLine);
            //    funcTParms.MoistureCurve1 = moisturecurve1.Value;

            //    ReadValue(moisturecurve2, currentLine);
            //    funcTParms.MoistureCurve2 = moisturecurve2.Value;

            //    ReadValue(moisturecurve3, currentLine);
            //    funcTParms.MoistureCurve3 = moisturecurve3.Value;

            //    ReadValue(moisturecurve4, currentLine);
            //    funcTParms.MoistureCurve4 = moisturecurve4.Value;

            //    ReadValue(wdr, currentLine);
            //    funcTParms.WoodDecayRate = wdr.Value;

            //    ReadValue(mwm, currentLine);
            //    funcTParms.MonthlyWoodMortality = mwm.Value;

            //    ReadValue(mortCurveShapeParm, currentLine);
            //    funcTParms.LongevityMortalityShape = mortCurveShapeParm.Value;

            //    ReadValue(foliageDropMonth, currentLine);
            //    funcTParms.FoliageDropMonth = foliageDropMonth.Value;

            //    ReadValue(coarseRootFraction, currentLine);
            //    funcTParms.CoarseRootFraction = coarseRootFraction.Value;

            //    ReadValue(fineRootFraction, currentLine);
            //    funcTParms.FineRootFraction = fineRootFraction.Value;

            //    //PlugIn.ModelCore.UI.WriteLine("PPRPTS2={0}.", parameters.FunctionalTypeTable[ln].PPRPTS2);

            //    CheckNoDataAfter("the " + fineRootFraction.Name + " column", currentLine);
            //    GetNextLine();
            //}
            
            //--------- Read In Fire Reductions Table ---------------------------
            PlugIn.ModelCore.UI.WriteLine("   Begin reading FIRE REDUCTION parameters.");
            ReadName(Names.FireReductionParameters);

            InputVar<int> frindex = new InputVar<int>("Fire Severity Index MUST = 1-10");
            InputVar<double> wred = new InputVar<double>("Coarse Litter Reduction");
            InputVar<double> lred = new InputVar<double>("Fine Litter Reduction");
            InputVar<double> live_wood_red = new InputVar<double>("Cohort Wood Reduction");
            InputVar<double> live_leaf_red = new InputVar<double>("Cohort Leaf Reduction");
            InputVar<double> som_red = new InputVar<double>("SOM Reduction");

            while (! AtEndOfInput && CurrentName != Names.HarvestReductionParameters)
// && CurrentName != Names.AgeOnlyDisturbanceParms)
            {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(frindex , currentLine);
                int ln = (int) frindex.Value.Actual;

                if(ln < 1 || ln > 10)
                    throw new InputValueException(frindex.Value.String,
                                              "The fire severity index:  {0} must be 1-10,",
                                              frindex.Value.String);


                FireReductions inputFireReduction = new FireReductions();  // ignoring severity = zero
                parameters.FireReductionsTable[ln] = inputFireReduction;

                ReadValue(wred, currentLine);
                inputFireReduction.CoarseLitterReduction = wred.Value;

                ReadValue(lred, currentLine);
                inputFireReduction.FineLitterReduction = lred.Value;

                ReadValue(live_wood_red, currentLine);
                inputFireReduction.CohortWoodReduction = live_wood_red.Value;

                ReadValue(live_leaf_red, currentLine);
                inputFireReduction.CohortLeafReduction = live_leaf_red.Value;

                ReadValue(som_red, currentLine);
                inputFireReduction.SOMReduction = som_red.Value;

                CheckNoDataAfter("the " + som_red.Name + " column", currentLine);

                GetNextLine();
            }

            //--------- Read In Harvest Reductions Table ---------------------------
            InputVar<string> hreds = new InputVar<string>("HarvestReductions");
            ReadName(Names.HarvestReductionParameters);
            PlugIn.ModelCore.UI.WriteLine("   Begin reading HARVEST REDUCTION parameters.");

            InputVar<string> prescriptionName = new InputVar<string>("Prescription");
            InputVar<double> wred_pr = new InputVar<double>("Coarse Litter Reduction");
            InputVar<double> lred_pr = new InputVar<double>("Fine Litter Reduction");
            InputVar<double> som_red_pr = new InputVar<double>("SOM Reduction");
            InputVar<double> cohortw_red_pr = new InputVar<double>("Cohort Wood Removal");
            InputVar<double> cohortl_red_pr = new InputVar<double>("Cohort Leaf Removal");


            while (!AtEndOfInput)
            {

                StringReader currentLine = new StringReader(CurrentLine);
                HarvestReductions harvReduction = new HarvestReductions();
                parameters.HarvestReductionsTable.Add(harvReduction);

                ReadValue(prescriptionName, currentLine);
                harvReduction.PrescriptionName = prescriptionName.Value;

                ReadValue(wred_pr, currentLine);
                harvReduction.CoarseLitterReduction = wred_pr.Value;

                ReadValue(lred_pr, currentLine);
                harvReduction.FineLitterReduction = lred_pr.Value;

                ReadValue(som_red_pr, currentLine);
                harvReduction.SOMReduction = som_red_pr.Value;

                ReadValue(cohortw_red_pr, currentLine);
                harvReduction.CohortWoodReduction = cohortw_red_pr.Value;

                ReadValue(cohortl_red_pr, currentLine);
                harvReduction.CohortLeafReduction = cohortl_red_pr.Value;

                GetNextLine();
            }


            return parameters; 
        }


        //---------------------------------------------------------------------

        /// <summary>
        /// Registers the appropriate method for reading input values.
        /// </summary>
        public static void RegisterForInputValues()
        {
            //Type.SetDescription<LayerType>("Litter Types");
            //InputValues.Register<LayerType>(LTParse);
            Type.SetDescription<WaterType>("Water Effect on Decomposition");
            InputValues.Register<WaterType>(WParse);

        }
        //---------------------------------------------------------------------
        public static WaterType WParse(string word)
        {
            if (word == "Linear")
                return WaterType.Linear;
            else if (word == "Ratio")
                return WaterType.Ratio;
            throw new System.FormatException("Valid names:  Linear, Ratio");
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Reads a species name from the current line, and verifies the name.
        /// </summary>
        private ISpecies ReadSpecies(StringReader currentLine)
        {
            ReadValue(speciesName, currentLine);
            ISpecies species = speciesDataset[speciesName.Value.Actual];
            if (species == null)
                throw new InputValueException(speciesName.Value.String,
                                              "{0} is not a species name.",
                                              speciesName.Value.String);
            int lineNumber;
            if (speciesLineNums.TryGetValue(species.Name, out lineNumber))
                throw new InputValueException(speciesName.Value.String,
                                              "The species {0} was previously used on line {1}",
                                              speciesName.Value.String, lineNumber);
            else
                speciesLineNums[species.Name] = LineNumber;
            return species;
        }

        private ISpecies ReadSpecies(string speciesName)
        {
            var species = speciesDataset[speciesName];
            if (species == null)
                throw new InputValueException(speciesName, $"{speciesName} is not a species name.");
            
            if (speciesLineNums.TryGetValue(species.Name, out int lineNumber))
                throw new InputValueException(speciesName, $"The species {speciesName} was previously used on line {lineNumber}");
            else
                speciesLineNums[species.Name] = LineNumber;
            return species;
        }

        private double ReadLightLAIAdjust(DataRow row)
        {
            try
            {
                double adjustLAI = System.Convert.ToDouble(row["LightLAIAdjust"]);
                return adjustLAI;
            }
            catch
            {
                return 1.0;  // value of 1.0 uses unadjusted Weibull distribution (area under the curve = 1)
            }
        }

        /// <summary>
        /// Reads ecoregion names as column headings
        /// </summary>
        //private List<IEcoregion> ReadEcoregions()
        //{
        //    if (AtEndOfInput)
        //        throw NewParseException("Expected a line with the names of 1 or more active ecoregions.");

        //    InputVar<string> ecoregionName = new InputVar<string>("Ecoregion");
        //    List<IEcoregion> ecoregions = new List<IEcoregion>();
        //    StringReader currentLine = new StringReader(CurrentLine);
        //    TextReader.SkipWhitespace(currentLine);
        //    while (currentLine.Peek() != -1) {
        //        ReadValue(ecoregionName, currentLine);
        //        IEcoregion ecoregion = ecoregionDataset[ecoregionName.Value.Actual];
        //        if (ecoregion == null)
        //            throw new InputValueException(ecoregionName.Value.String,
        //                                          "{0} is not an ecoregion name.",
        //                                          ecoregionName.Value.String);
        //        if (! ecoregion.Active)
        //            throw new InputValueException(ecoregionName.Value.String,
        //                                          "{0} is not an active ecoregion",
        //                                          ecoregionName.Value.String);
        //        if (ecoregions.Contains(ecoregion))
        //            throw new InputValueException(ecoregionName.Value.String,
        //                                          "The ecoregion {0} appears more than once.",
        //                                          ecoregionName.Value.String);
        //        ecoregions.Add(ecoregion);
        //        TextReader.SkipWhitespace(currentLine);
        //    }
        //    GetNextLine();

        //    return ecoregions;
        //}

        ////---------------------------------------------------------------------

        //private IEcoregion GetEcoregion(InputValue<string>      ecoregionName,
        //                                Dictionary<string, int> lineNumbers)
        //{
        //    IEcoregion ecoregion = ecoregionDataset[ecoregionName.Actual];
        //    if (ecoregion == null)
        //        throw new InputValueException(ecoregionName.String,
        //                                      "{0} is not an ecoregion name.",
        //                                      ecoregionName.String);
        //    int lineNumber;
        //    if (lineNumbers.TryGetValue(ecoregion.Name, out lineNumber))
        //        throw new InputValueException(ecoregionName.String,
        //                                      "The ecoregion {0} was previously used on line {1}",
        //                                      ecoregionName.String, lineNumber);
        //    else
        //        lineNumbers[ecoregion.Name] = LineNumber;

        //    return ecoregion;
        //}
    }
}
