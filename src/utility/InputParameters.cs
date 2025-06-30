//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.Parameters;
using System.Collections.Generic;
using System.Diagnostics;

namespace Landis.Extension.Succession.DGS
{
    /// <summary>
    /// The parameters for biomass succession.
    /// </summary>
    public class InputParameters
        : IInputParameters
    {
        private int _timestep;
        private string _initCommunities;
        private string _communitiesMap;
        private string _soilDepthMapName;
        private string _soilDrainMapName;
        private string _soilBaseFlowMapName;
        private string _soilStormFlowMapName;
        private string _soilFieldCapacityMapName;
        private string _soilWiltingPointMapName;
        private string _soilPercentSandMapName;
        private string _soilPercentClayMapName;
        private string _initialDeadSurfaceMapName;
        private string _initialDeadSoilMapName;
        private string _soilBulkDensityMapName;
        private string _soilParticleDensityMapName;
        private string _initialSOC_PrimaryMapName;
        private string _initialSON_PrimaryMapName;

        public double _probEstablishAdjust;
        //private double initFineFuels;
        //private double initMineralN;
        //private double initMicrobialC;
        //private double initMicrobialN;
        //private double initEnzymeConc;
        //private double aes_som_depoly;
        //private double ae_doc_uptake;
        //private double ec_som_depoly;
        //private double ec_uptake;
        //private double frac_som_unprotect;
        //private double cn_enzymes;
        //private double km_dep;
        //private double km_upt;
        //private double r_ecloss;
        //private double r_death;
        //private double c_use_efficiency;
        //private double p_enz_SOC;
        //private double pconst;
        //private double qconst;
        //private double mic_to_som;
        //private double km_o2;
        //private double dgas;
        //private double dliq;
        //private double o2airfrac;
        //private double initDOCFraction;
        //private double initDONFraction;        

        //private double fractionLitterDecayToDOC;
        //private double soilmoistA;
        //private double soilmoistB;

        //private FunctionalTypeTable _functionalTypes;

        public int Timestep { get { return _timestep; } set { if (value < 0) throw new InputValueException(value.ToString(), "Timestep must be > or = 0"); _timestep = value; } } 
        public SeedingAlgorithms SeedAlgorithm { get; set; }
        public string InitialCommunities { get { return _initCommunities; } set { if (value != null) { ValidatePath(value); } _initCommunities = value; } }
        public string InitialCommunitiesMap { get { return _communitiesMap; } set { if (value != null) { ValidatePath(value); } _communitiesMap = value; } }
        public string ClimateConfigFile { get; set; }
        public string ShawGiplConfigFile { get; set; }

        /// <summary>
        /// Determines whether months are simulated 0 - 12 (calibration mode) or
        /// 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5 (normal mode with disturbance at June 30).
        /// </summary>
        public bool CalibrateMode { get; set; }

        public bool SmokeModelOutputs { get; set; }

        /// <summary>
        /// Determines whether moisture effects on decomposition follow a linear or ratio calculation.
        /// </summary>
        public WaterType WType { get; set; }

        /// <summary>
        /// Adjust probability of establishment due to variable time step.  A multiplier.
        /// </summary>
        public double ProbEstablishAdjustment { get { return _probEstablishAdjust; } set { if (value < 0.0 || value > 1.0) throw new InputValueException(value.ToString(), "Probability of adjustment factor must be > 0.0 and < 1"); _probEstablishAdjust = value; } }

        public double AtmosNslope { get; set; }
        public double AtmosNintercept { get; set; }

        ///// <summary>
        ///// Functional type parameters.
        ///// </summary>
        //public FunctionalTypeTable FunctionalTypes { get { return _functionalTypes; } set { _functionalTypes = value; } }

        /// <summary>
        /// Fire reduction of leaf and wood litter parameters.
        /// </summary>
        public FireReductions[] FireReductionsTable { get; set; } = new FireReductions[11];

        /// <summary>
        /// Harvest reduction of leaf and wood litter parameters.
        /// </summary>
        public List<HarvestReductions> HarvestReductionsTable { get; set; } = new List<HarvestReductions>();

        public double[] MaximumShadeLAI { get; } = new double[6];

        //public Landis.Library.Parameters.Species.AuxParm<int> SppFunctionalType { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<int> GDDmin { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<int> GDDmax { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<int> MinJanTemp { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> MaxDrought { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> LightLAIShape { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> LightLAIScale { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> LightLAILocation { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> LightLAIAdjust { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<bool> AdventRoots { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<bool> NFixer { get; set; }

        /// <summary>
        /// Can the species resprout epicormically following a fire?
        /// </summary>
        public Landis.Library.Parameters.Species.AuxParm<bool> Epicormic { get; set; }

        public Landis.Library.Parameters.Species.AuxParm<double> KLAI { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> MaxLAI { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> CompLimit { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> LeafBiomassToLAI { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> LeafLongevity { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<int> FoliageDropMonth { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<int> MaxANPP { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<int> MaxBiomass { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> TemperatureCurve1 { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> TemperatureCurve2 { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> TemperatureCurve3 { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> TemperatureCurve4 { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> MoistureCurve1 { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> MoistureCurve2 { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> MoistureCurve3 { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> MoistureCurve4 { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> FractionANPPtoLeaf { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> CoarseRootFraction { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> FineRootFraction { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<int> RootingDepth { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> LongevityMortalityShape { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<int> FireTolerance { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> LeafLignin { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> FineRootLignin { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> WoodLignin { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> CoarseRootLignin { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> LeafCN { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> FineRootCN { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> WoodCN { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> CoarseRootCN { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> FoliageLitterCN { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> WoodDecayRate { get; set; }
        public Landis.Library.Parameters.Species.AuxParm<double> MonthlyWoodMortality { get; set; }

        /// <summary>
        /// Definitions of sufficient light probabilities.
        /// </summary>
        public List<ISufficientLight> LightClassProbabilities { get; } = new List<ISufficientLight>();

        //---------------------------------------------------------------------
        public double Latitude { get; set; }

        public double InitialFineFuels { get; set; }
        public double InitialMineralN { get; set; }
        public double InitialMicrobialC { get; set; }
        public double InitialMicrobialN { get; set; }
        public double InitialEnzymeConc { get; set; }
        public double ActEnergySOMDepoly { get; set; }
        public double ActEnergyDOCUptake { get; set; }
        public double ExpConstSOMDepoly { get; set; }
        public double ExpConstDOCUptake { get; set; }
        public double FractionSOMUnprotect { get; set; }
        public double CNEnzymes { get; set; }
        public double KmSOMDepoly { get; set; }
        public double KmDOCUptake { get; set; }
        public double EnzTurnRate { get; set; }
        public double MicrobialTurnRate { get; set; }
        public double CarbonUseEfficiency { get; set; }
        public double PropEnzymeSOM { get; set; }
        public double PropCEnzymeProduction { get; set; }
        public double PropNEnzymeProduction { get; set; }
        public double FractDeadMicrobialBiomassSOM { get; set; }
        public double MMConstantO2 { get; set; }
        public double DiffConstantO2 { get; set; }
        public double DiffConstantSOMLiquid { get; set; }
        public double FractionVolumeO2 { get; set; }
        public double InitialDOCFraction { get; set; }
        public double InitialDONFraction { get; set; }
        public double FractionLitterDecayToDOC { get; set; }
        //public double SoilMoistureA { get; set; }
        //public double SoilMoistureB { get; set; }
        public double DenitrificationRate { get; set; }

        public string SoilDepthMapName { get { return _soilDepthMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, $"\"{path}\" is not a valid path."); _soilDepthMapName = value; } }

        public string SoilDrainMapName { get { return _soilDrainMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, $"\"{path}\" is not a valid path."); _soilDrainMapName = value; } }

        public string SoilBaseFlowMapName { get { return _soilBaseFlowMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, $"\"{path}\" is not a valid path."); _soilBaseFlowMapName = value; } }

        public string SoilStormFlowMapName { get { return _soilStormFlowMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, "\"{0}\" is not a valid path.", path); _soilStormFlowMapName = value; } }

        public string SoilFieldCapacityMapName { get { return _soilFieldCapacityMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, "\"{0}\" is not a valid path.", path); _soilFieldCapacityMapName = value; } }

        public string SoilWiltingPointMapName { get { return _soilWiltingPointMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, "\"{0}\" is not a valid path.", path); _soilWiltingPointMapName = value; } }

        public string SoilPercentSandMapName { get { return _soilPercentSandMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, "\"{0}\" is not a valid path.", path); _soilPercentSandMapName = value; } }

        public string SoilPercentClayMapName { get { return _soilPercentClayMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, "\"{0}\" is not a valid path.", path); _soilPercentClayMapName = value; } }

        public string SoilBulkDensityMapName { get { return _soilBulkDensityMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, "\"{0}\" is not a valid path.", path); _soilBulkDensityMapName = value; } }

        public string SoilParticleDensityMapName { get { return _soilParticleDensityMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, "\"{0}\" is not a valid path.", path); _soilParticleDensityMapName = value; } }

        public string InitialSOC_PrimaryMapName { get { return _initialSOC_PrimaryMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, "\"{0}\" is not a valid path.", path); _initialSOC_PrimaryMapName = value; } }

        public string InitialSON_PrimaryMapName { get { return _initialSON_PrimaryMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, "\"{0}\" is not a valid path.", path); _initialSON_PrimaryMapName = value; } }

        public string InitialDeadSurfaceMapName { get { return _initialDeadSurfaceMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, "\"{0}\" is not a valid path.", path); _initialDeadSurfaceMapName = value; } }

        public string InitialDeadSoilMapName { get { return _initialDeadSoilMapName; } set { string path = value; if (path.Trim(null).Length == 0) throw new InputValueException(path, "\"{0}\" is not a valid path.", path); _initialDeadSoilMapName = value; } }

        public void SetMaximumShadeLAI(byte shadeClass,
                                          //IEcoregion             ecoregion,
                                          InputValue<double> newValue)
        {
            Debug.Assert(1 <= shadeClass && shadeClass <= 5);
            //Debug.Assert(ecoregion != null);
            if (newValue != null)
            {
                if (newValue.Actual < 0.0 || newValue.Actual > 20)
                    throw new InputValueException(newValue.String,
                                                  "{0} is not between 0 and 20", newValue.String);
            }

            MaximumShadeLAI[shadeClass] = newValue;
            //minRelativeBiomass[shadeClass][ecoregion] = newValue;
        }
        //---------------------------------------------------------------------

        //public void SetFunctionalType(ISpecies species,
        //                             InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    sppFunctionalType[species] = CheckBiomassParm(newValue, 0, 100);
        //}
        //---------------------------------------------------------------------

        //public void SetNFixer(ISpecies           species,
        //                             InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    nTolerance[species] = CheckBiomassParm(newValue, 1, 4);
        //}

        //---------------------------------------------------------------------

        //public void SetGDDmin(ISpecies species,
        //                             InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    gddMin[species] = CheckBiomassParm(newValue, 1, 4000);
        //}

        public void SetGDDmin(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            GDDmin[species] = VerifyRange(newValue, 1, 4000, "GDDMin");
        }

        //---------------------------------------------------------------------

        //public void SetGDDmax(ISpecies species,
        //                             InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    gddMax[species] = CheckBiomassParm(newValue, 500, 7000);
        //}
        public void SetGDDmax(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            GDDmax[species] = VerifyRange(newValue, 500, 7000, "GDDmax");
        }

        //---------------------------------------------------------------------

        //public void SetMinJanTemp(ISpecies species,
        //                             InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    minJanTemp[species] = CheckBiomassParm(newValue, -60, 20);
        //}
        public void SetMinJanTemp(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            MinJanTemp[species] = VerifyRange(newValue, -60, 20, "MinJanTemp");
        }

        //---------------------------------------------------------------------

        //public void SetMaxDrought(ISpecies species,
        //                             InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    maxDrought[species] = CheckBiomassParm(newValue, 0.0, 1.0);
        //}
        public void SetMaxDrought(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            MaxDrought[species] = VerifyRange(newValue, 0.0, 1.0, "MaxDrought");
        }

        public void SetKLAI(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            KLAI[species] = VerifyRange(newValue, 1.0, 50000.0, "KLAI");
        }

        public void SetMaxLAI(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            MaxLAI[species] = VerifyRange(newValue, 0.0, 50.0, "MaxLAI");
        }

        public void SetCompLimit(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            CompLimit[species] = VerifyRange(newValue, 0.00000001, 5.0, "CompLimit");
        }

        public void SetLeafBiomassToLAI(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            LeafBiomassToLAI[species] = VerifyRange(newValue, -3.0, 1000.0, "LeafBiomassToLAI");
        }

        //---------------------------------------------------------------------

        //public void SetLeafLongevity(ISpecies species,
        //                             InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    leafLongevity[species] = CheckBiomassParm(newValue, 1.0, 10.0);
        //}
        public void SetLeafLongevity(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            LeafLongevity[species] = VerifyRange(newValue, 1.0, 10.0, "LeafLongevity");
        }

        public void SetFoliageDropMonth(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            FoliageDropMonth[species] = VerifyRange(newValue, 1, 12, "FoliageDropMonth");
        }

        //---------------------------------------------------------------------

        //public void SetLeafLignin(ISpecies species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    leafLignin[species] = CheckBiomassParm(newValue, 0.0, 0.4);
        //}
        public void SetLeafLignin(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            LeafLignin[species] = VerifyRange(newValue, 0.0, 0.4, "LeafLignin");
        }
        //---------------------------------------------------------------------

        //public void SetWoodLignin(ISpecies species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    woodLignin[species] = CheckBiomassParm(newValue, 0.0, 0.4);
        //}
        public void SetWoodLignin(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            WoodLignin[species] = VerifyRange(newValue, 0.0, 0.4, "WoodLignin");
        }
        //---------------------------------------------------------------------

        //public void SetCoarseRootLignin(ISpecies species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    coarseRootLignin[species] = CheckBiomassParm(newValue, 0.0, 0.4);
        //}
        public void SetCoarseRootLignin(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            CoarseRootLignin[species] = VerifyRange(newValue, 0.0, 0.4, "CourseRootLignin");
        }
        //---------------------------------------------------------------------

        //public void SetFineRootLignin(ISpecies species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    fineRootLignin[species] = CheckBiomassParm(newValue, 0.0, 0.4);
        //}
        public void SetFineRootLignin(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            FineRootLignin[species] = VerifyRange(newValue, 0.0, 0.4, "FineRootLignin");
        }
        //---------------------------------------------------------------------

        //public void SetLeafCN(ISpecies species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    leafCN[species] = CheckBiomassParm(newValue, 5.0, 100.0);
        //}
        public void SetLeafCN(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            LeafCN[species] = VerifyRange(newValue, 5.0, 100.0, "LeafCN");
        }
        //---------------------------------------------------------------------

        //public void SetWoodCN(ISpecies species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    //woodCN[species] = CheckBiomassParm(newValue, 5.0, 600.0);
        //    woodCN[species] = CheckBiomassParm(newValue, 5.0, 900.0);
        //}
        public void SetWoodCN(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            WoodCN[species] = VerifyRange(newValue, 5.0, 900.0, "WoodCN");
        }
        //---------------------------------------------------------------------

        //public void SetCoarseRootCN(ISpecies species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    coarseRootCN[species] = CheckBiomassParm(newValue, 5.0, 500.0);
        //}
        public void SetCoarseRootCN(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            CoarseRootCN[species] = VerifyRange(newValue, 5.0, 500.0, "CourseRootCN");
        }
        //---------------------------------------------------------------------

        //public void SetFoliageLitterCN(ISpecies species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    foliageLitterCN[species] = CheckBiomassParm(newValue, 5.0, 100.0);
        //}
        public void SetFoliageLitterCN(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            FoliageLitterCN[species] = VerifyRange(newValue, 5.0, 100.0, "FoliarLitterCN");
        }
        //---------------------------------------------------------------------

        //public void SetFineRootCN(ISpecies species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    fineRootCN[species] = CheckBiomassParm(newValue, 5.0, 100.0);
        //}
        public void SetFineRootCN(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            FineRootCN[species] = VerifyRange(newValue, 5.0, 100.0, "FineRootCN");
        }
        //---------------------------------------------------------------------

        //public void SetRootingDepth(ISpecies species,
        //                                 InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    RootingDepth[species] = CheckBiomassParm(newValue, 0, 5000);
        //}

        public void SetMaxANPP(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            MaxANPP[species] = VerifyRange(newValue, 2, 1000, "MaxANPP");
        }

        public void SetMaxBiomass(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            MaxBiomass[species] = VerifyRange(newValue, 2, 300000, "MaxBiomass");
        }

        public void SetTemperatureCurve1(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            TemperatureCurve1[species] = VerifyRange(newValue, 8, 40, "TemperatureCurve1");
        }

        public void SetTemperatureCurve2(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            TemperatureCurve2[species] = VerifyRange(newValue, 20, 500, "TemperatureCurve2");
        }

        public void SetTemperatureCurve3(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            TemperatureCurve3[species] = VerifyRange(newValue, 0, 500, "TemperatureCurve3");
        }

        public void SetTemperatureCurve4(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            TemperatureCurve4[species] = VerifyRange(newValue, 0, 500, "TemperatureCurve4");
        }

        public void SetFractionANPPtoLeaf(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            FractionANPPtoLeaf[species] = VerifyRange(newValue, 0.1, 1.0, "FractionANPPtoLeaf");
        }

        public void SetCoarseRootFraction(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            CoarseRootFraction[species] = VerifyRange(newValue, 0.0, 1.0, "CoarseRootFraction");
        }
        public void SetFineRootFraction(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            FineRootFraction[species] = VerifyRange(newValue, 0.0, 1.0, "FineRootFraction");
        }

        public void SetRootingDepth(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            RootingDepth[species] = VerifyRange(newValue, 0, 5000, "RootDepth");
        }

        public void SetMortalityShapeCurve(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            LongevityMortalityShape[species] = VerifyRange(newValue, 2.0, 25.0, "LongevityMortalityShape");
        }

        public void SetFireTolerance(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            FireTolerance[species] = VerifyRange(newValue, 0, 1000, "FireTolerance");
        }

        public void SetWoodDecayRate(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            WoodDecayRate[species] = VerifyRange(newValue, 0.0, 2.0, "WoodDecayRate");
        }

        public void SetMonthlyWoodMortality(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            MonthlyWoodMortality[species] = VerifyRange(newValue, 0.0, 1.0, "MonthlyWoodMortality");
        }


        //---------------------------------------------------------------------

        //public void SetFireTolerance(ISpecies species, InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    FireTolerance[species] = CheckBiomassParm(newValue, 0, byte.MaxValue);
        //}
        ////---------------------------------------------------------------------

        //public void SetShadeTolerance(ISpecies species, InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    ShadeTolerance[species] = CheckBiomassParm(newValue, 0, byte.MaxValue);
        //}
        //---------------------------------------------------------------------

        //public void SetLightLAIShape(ISpecies species, InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    lightLAIShape[species] = CheckBiomassParm(newValue, 0.0, 10.0);
        //}

        //public void SetLightLAIScale(ISpecies species, InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    lightLAIScale[species] = CheckBiomassParm(newValue, 0.0, 1000.0); //Scale parameter can be high for some shade-tolerant spp
        //}

        //public void SetLightLAILocation(ISpecies species, InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    lightLAILocation[species] = CheckBiomassParm(newValue, 0.0, 1);
        //}

        //public void SetLightLAIAdjust(ISpecies species, InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    lightLAIAdjust[species] = CheckBiomassParm(newValue, 0.0, 100.0);
        //}

        public void SetLightLAIShape(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            LightLAIShape[species] = VerifyRange(newValue, 0.0, 10.0, "LightLAIShape");
        }

        public void SetLightLAIScale(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            LightLAIScale[species] = VerifyRange(newValue, 0.0, 1000.0, "LightLAIScale"); //Scale parameter can be high for some shade-tolerant spp
        }

        public void SetLightLAILocation(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            LightLAILocation[species] = VerifyRange(newValue, 0.0, 1, "LightLAILocation");
        }

        public void SetLightLAIAdjust(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            LightLAIAdjust[species] = VerifyRange(newValue, 0.0, 100.0, "LightLAIAdjust");
        }

        public void SetAtmosNslope(InputValue<double> newValue)
        {
            AtmosNslope = CheckBiomassParm(newValue, -1.0, 2.0);
        }
        //---------------------------------------------------------------------
        public void SetAtmosNintercept(InputValue<double> newValue)
        {
            AtmosNintercept = CheckBiomassParm(newValue, -1.0, 2.0);
        }
        //---------------------------------------------------------------------
        public void SetLatitude(InputValue<double> newValue)
        {
            //Debug.Assert(ecoregion != null);
            Latitude = CheckBiomassParm(newValue, 0.0, 80.0);
        }
       
        //---------------------------------------------------------------------
        public void SetInitFineFuels(InputValue<double> newValue)
        {
            InitialFineFuels = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------
        public void SetInitMineralN(InputValue<double> newValue)
        {
            InitialMineralN = CheckBiomassParm(newValue, 0.0, 50.0);
        }

        //---------------------------------------------------------------------
        public void SetInitMicrobialC(InputValue<double> newValue)
        {
            InitialMicrobialC = CheckBiomassParm(newValue, 0.0, 5.0);
        }

        //---------------------------------------------------------------------
        public void SetInitMicrobialN(InputValue<double> newValue)
        {
            InitialMicrobialN = CheckBiomassParm(newValue, 0.0, 5.0);
        }

        //---------------------------------------------------------------------
        public void SetInitEnzymeConc(InputValue<double> newValue)
        {
            InitialEnzymeConc = CheckBiomassParm(newValue, 0.0, 5.0);
        }

        //---------------------------------------------------------------------
        public void SetActEnergySOMDepoly(InputValue<double> newValue)
        {
            ActEnergySOMDepoly = CheckBiomassParm(newValue, 0.0, 200.0);
        }
        //---------------------------------------------------------------------
        public void SetActEnergyDOCUptake(InputValue<double> newValue)
        {
            ActEnergyDOCUptake = CheckBiomassParm(newValue, 0.0, 200.0);
        }
        //---------------------------------------------------------------------
        public void SetECSOMDepoly(InputValue<double> newValue)
        {
            ExpConstSOMDepoly = CheckBiomassParm(newValue, 0.0, 200000000000);
        }
        //---------------------------------------------------------------------
        public void SetExpConstDOCUptake(InputValue<double> newValue)
        {
            ExpConstDOCUptake = CheckBiomassParm(newValue, 0.0, 200000000000.0);
        }
        //---------------------------------------------------------------------
        public void SetFractionSOMUnprotect(InputValue<double> newValue)
        {
            FractionSOMUnprotect = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        public void SetCNEnzymes(InputValue<double> newValue)
        {
            CNEnzymes = CheckBiomassParm(newValue, 0.0, 5.0);
        }
        //---------------------------------------------------------------------
        public void SetKmSOMDepoly(InputValue<double> newValue)
        {
            KmSOMDepoly = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetKmDOCUptake(InputValue<double> newValue)
        {
            KmDOCUptake = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        //---------------------------------------------------------------------

        public void SetEnzymeTurnRate(InputValue<double> newValue)
        {
            EnzTurnRate = CheckBiomassParm(newValue, 0.0, 0.1);
        }
        //---------------------------------------------------------------------

        public void SetMicrobialTurnRate(InputValue<double> newValue)
        {
            MicrobialTurnRate = CheckBiomassParm(newValue, 0.0, 0.1);
        }
        //---------------------------------------------------------------------

        public void SetCarbonUseEfficiency(InputValue<double> newValue)
        {
            CarbonUseEfficiency = CheckBiomassParm(newValue, 0.0, 0.9);
        }
        //---------------------------------------------------------------------

        public void SetPropEnzymeSOM(InputValue<double> newValue)
        {
            PropEnzymeSOM = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetPropCEnzymeProduction(InputValue<double> newValue)
        {
            PropCEnzymeProduction = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetPropNEnzymeProduction(InputValue<double> newValue)
        {
            PropNEnzymeProduction = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------
        public void SetFractDeadMicrobialBiomassSOM(InputValue<double> newValue)
        {
            FractDeadMicrobialBiomassSOM = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------
        public void SetMMConstantO2(InputValue<double> newValue)
        {
            MMConstantO2 = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------
        public void SetDiffConstantO2(InputValue<double> newValue)
        {
            DiffConstantO2 = CheckBiomassParm(newValue, 0.0, 5.0);
        }
        //---------------------------------------------------------------------

        public void SetDiffConstantSOMLiquid(InputValue<double> newValue)
        {
            DiffConstantSOMLiquid = CheckBiomassParm(newValue, 0.0, 5.0);
        }

        //---------------------------------------------------------------------
        public void SetFractionVolumeO2(InputValue<double> newValue)
        {
            FractionVolumeO2 = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        public void SetInitDOCFraction(InputValue<double> newValue)
        {
            InitialDOCFraction = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        public void SetInitDONFraction(InputValue<double> newValue)
        {
            InitialDONFraction = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        public void SetFractionDOC(InputValue<double> newValue)
        {
            FractionLitterDecayToDOC = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        //public void SetSoilMoistureA (InputValue<double> newValue)
        //{
        //    SoilMoistureA = CheckBiomassParm(newValue, 0.0, 50.0);
        //}

        //public void SetSoilMoistureB (InputValue<double> newValue)
        //{
        //    SoilMoistureB = CheckBiomassParm(newValue, 0.0, 50.0);
        //}

        public void SetDenitrif(InputValue<double> newValue)
        {
            DenitrificationRate = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        public InputParameters(ISpeciesDataset speciesDataset, int litterCnt, int functionalCnt)
        {
            //FunctionalTypes = new FunctionalTypeTable(functionalCnt);

            //SppFunctionalType = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            GDDmin = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            GDDmax = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            MinJanTemp = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            MaxDrought = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            LightLAIShape = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            LightLAIScale = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            LightLAILocation = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            LightLAIAdjust = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            AdventRoots = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            NFixer = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            Epicormic = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            KLAI = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            MaxLAI = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            CompLimit = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            LeafBiomassToLAI = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            LeafLongevity = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            FoliageDropMonth = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            MaxANPP = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            MaxBiomass = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            TemperatureCurve1 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            TemperatureCurve2 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            TemperatureCurve3 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            TemperatureCurve4 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            MoistureCurve1 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            MoistureCurve2 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            MoistureCurve3 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            MoistureCurve4 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            FractionANPPtoLeaf = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            CoarseRootFraction = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            FineRootFraction = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            RootingDepth = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            LongevityMortalityShape = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            FireTolerance = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            LeafLignin = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            FineRootLignin = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            WoodLignin = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            CoarseRootLignin = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            LeafCN = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            FineRootCN = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            WoodCN = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            CoarseRootCN = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            FoliageLitterCN = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            WoodDecayRate = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            MonthlyWoodMortality = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
        }


        public static int VerifyRange(int newValue, int minValue, int maxValue, string parameterName)
        {
            if (newValue < minValue || newValue > maxValue)
                throw new InputValueException(newValue.ToString(), $"{parameterName} {newValue} is not between {minValue:0.0} and {maxValue:0.0}");
            
            return newValue;
        }

        public static double VerifyRange(double newValue, double minValue, double maxValue, string parameterName)
        {
            if (newValue < minValue || newValue > maxValue)
                throw new InputValueException(newValue.ToString(), $"{parameterName} {newValue} is not between {minValue:0.0} and {maxValue:0.0}");

            return newValue;
        }

        private double CheckBiomassParm(InputValue<double> newValue,
                                                    double minValue,
                                                    double maxValue)
        {
            if (newValue != null)
            {
                if (newValue.Actual < minValue || newValue.Actual > maxValue)
                    throw new InputValueException(newValue.String,
                                                  "{0} is not between {1:0.0} and {2:0.0}",
                                                  newValue.String, minValue, maxValue);
            }
            return newValue.Actual;
        }
        //---------------------------------------------------------------------

        private int CheckBiomassParm(InputValue<int> newValue,
                                                    int minValue,
                                                    int maxValue)
        {
            if (newValue != null)
            {
                if (newValue.Actual < minValue || newValue.Actual > maxValue)
                    throw new InputValueException(newValue.String,
                                                  "{0} is not between {1:0.0} and {2:0.0}",
                                                  newValue.String, minValue, maxValue);
            }
            return newValue.Actual;
        }

        //---------------------------------------------------------------------

        private void ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new InputValueException();
            if (path.Trim(null).Length == 0)
                throw new InputValueException(path,
                                              "\"{0}\" is not a valid path.",
                                              path);
        }
    }
}


