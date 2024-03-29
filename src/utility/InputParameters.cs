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
        private int timestep;
        private SeedingAlgorithms seedAlg;

        private string climateConfigFile;
        private string shawGiplConfigFile;
        private string initCommunities;
        private string communitiesMap;
        private string soilDepthMapName;
        private string soilDrainMapName;
        private string soilBaseFlowMapName;
        private string soilStormFlowMapName;
        private string soilFieldCapacityMapName;
        private string soilWiltingPointMapName;
        private string soilPercentSandMapName;
        private string soilPercentClayMapName;
        private string initialDeadSurfaceMapName;
        private string initialDeadSoilMapName;
        private string soilBulkDensityMapName;
        private string soilParticleDensityMapName;
        private string initialSOC_PrimaryMapName;
        private string initialSON_PrimaryMapName;
        //private string initialSOM1CSurfaceMapName;
        //private string initialSOM1NSurfaceMapName;
        //private string initialSOM2CMapName;
        //private string initialSOM2NMapName;
        //private string initialSOM3CMapName;
        //private string initialSOM3NMapName;

        private bool calibrateMode;
        private bool smokeModelOutputs;
        public WaterType wtype;
        public double probEstablishAdjust;
        private double atmosNslope;
        private double atmosNintercept;
        private double latitude;
        private double denitrif;
        private double[] maximumShadeLAI;
        private double initFineFuels;
        private double initMineralN;
        private double initMicrobialC;
        private double initMicrobialN;
        private double initEnzymeConc;
        private double aes_som_depoly;
        private double ae_doc_uptake;
        private double ec_som_depoly;
        private double ec_uptake;
        private double frac_som_unprotect;
        private double cn_enzymes;
        private double km_dep;
        private double km_upt;
        private double r_ecloss;
        private double r_death;
        private double c_use_efficiency;
        private double p_enz_SOC;
        private double pconst;
        private double qconst;
        private double mic_to_som;
        private double km_o2;
        private double dgas;
        private double dliq;
        private double o2airfrac;
        private double initDOCFraction;
        private double initDONFraction;        
        //private double initDOC;

        private double fractionLitterDecayToDOC;
        private double soilmoistA;
        private double soilmoistB;
        //private double decayRateSurf;
        //private double decayRateSOM1;
        //private double decayRateSOM2;
        //private double decayRateSOM3;

        private ISpeciesDataset speciesDataset;

        private FunctionalTypeTable functionalTypes;
        private FireReductions[] fireReductionsTable;
        private List<HarvestReductions> harvestReductionsTable;

        private Landis.Library.Parameters.Species.AuxParm<int> sppFunctionalType;
        private Landis.Library.Parameters.Species.AuxParm<bool> nFixer;
        private Landis.Library.Parameters.Species.AuxParm<bool> adventRoots;
        private Landis.Library.Parameters.Species.AuxParm<int> gddMin;
        private Landis.Library.Parameters.Species.AuxParm<int> gddMax;
        private Landis.Library.Parameters.Species.AuxParm<int> minJanTemp;
        private Landis.Library.Parameters.Species.AuxParm<double> maxDrought;
        private Landis.Library.Parameters.Species.AuxParm<double> leafLongevity;
        private Landis.Library.Parameters.Species.AuxParm<bool> epicormic;
        private Landis.Library.Parameters.Species.AuxParm<double> leafLignin;
        private Landis.Library.Parameters.Species.AuxParm<double> woodLignin;
        private Landis.Library.Parameters.Species.AuxParm<double> coarseRootLignin;
        private Landis.Library.Parameters.Species.AuxParm<double> fineRootLignin;
        private Landis.Library.Parameters.Species.AuxParm<double> leafCN;
        private Landis.Library.Parameters.Species.AuxParm<double> woodCN;
        private Landis.Library.Parameters.Species.AuxParm<double> coarseRootCN;
        private Landis.Library.Parameters.Species.AuxParm<double> foliageLitterCN;
        private Landis.Library.Parameters.Species.AuxParm<double> fineRootCN;
        private Landis.Library.Parameters.Species.AuxParm<int> rootingDepth;
        private Landis.Library.Parameters.Species.AuxParm<int> maxANPP;
        private Landis.Library.Parameters.Species.AuxParm<int> maxBiomass;

        private List<ISufficientLight> sufficientLight;


        //---------------------------------------------------------------------
        /// <summary>
        /// Timestep (years)
        /// </summary>
        public int Timestep
        {
            get
            {
                return timestep;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Timestep must be > or = 0");
                timestep = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Seeding algorithm
        /// </summary>
        public SeedingAlgorithms SeedAlgorithm
        {
            get
            {
                return seedAlg;
            }
            set
            {
                seedAlg = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the file with the initial communities' definitions.
        /// </summary>
        public string InitialCommunities
        {
            get
            {
                return initCommunities;
            }

            set
            {
                if (value != null)
                {
                    ValidatePath(value);
                }
                initCommunities = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the raster file showing where the initial communities are.
        /// </summary>
        public string InitialCommunitiesMap
        {
            get
            {
                return communitiesMap;
            }

            set
            {
                if (value != null)
                {
                    ValidatePath(value);
                }
                communitiesMap = value;
            }
        }

        //---------------------------------------------------------------------
        public string ClimateConfigFile
        {
            get
            {
                return climateConfigFile;
            }
            set
            {

                climateConfigFile = value;
            }
        }

        public string ShawGiplConfigFile { get => shawGiplConfigFile; set => shawGiplConfigFile = value; }

        //---------------------------------------------------------------------
        /// <summary>
        /// Determines whether months are simulated 0 - 12 (calibration mode) or
        /// 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5 (normal mode with disturbance at June 30).
        /// </summary>
        public bool CalibrateMode
        {
            get
            {
                return calibrateMode;
            }
            set
            {
                calibrateMode = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// </summary>
        public bool SmokeModelOutputs
        {
            get
            {
                return smokeModelOutputs;
            }
            set
            {
                smokeModelOutputs = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Determines whether moisture effects on decomposition follow a linear or ratio calculation.
        /// </summary>
        public WaterType WType
        {
            get
            {
                return wtype;
            }
            set
            {
                wtype = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Adjust probability of establishment due to variable time step.  A multiplier.
        /// </summary>
        public double ProbEstablishAdjustment
        {
            get
            {
                return probEstablishAdjust;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Probability of adjustment factor must be > 0.0 and < 1");
                probEstablishAdjust = value;
            }
        }
        //---------------------------------------------------------------------
        public double AtmosNslope
        {
            get
            {
                return atmosNslope;
            }
        }
        //---------------------------------------------------------------------
        public double AtmosNintercept
        {
            get
            {
                return atmosNintercept;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Functional type parameters.
        /// </summary>
        public FunctionalTypeTable FunctionalTypes
        {
            get
            {
                return functionalTypes;
            }
            set
            {
                functionalTypes = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Fire reduction of leaf and wood litter parameters.
        /// </summary>
        public FireReductions[] FireReductionsTable
        {
            get
            {
                return fireReductionsTable;
            }
            set
            {
                fireReductionsTable = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Harvest reduction of leaf and wood litter parameters.
        /// </summary>
        public List<HarvestReductions> HarvestReductionsTable
        {
            get
            {
                return harvestReductionsTable;
            }
            set
            {
                harvestReductionsTable = value;
            }
        }
        //---------------------------------------------------------------------
        public double[] MaximumShadeLAI
        {
            get
            {
                return maximumShadeLAI;
            }
        }


        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<int> SppFunctionalType { get { return sppFunctionalType; } }
        public Landis.Library.Parameters.Species.AuxParm<bool> NFixer { get { return nFixer; } }
        public Landis.Library.Parameters.Species.AuxParm<bool> AdventRoots { get { return adventRoots; } }
        public Landis.Library.Parameters.Species.AuxParm<int> GDDmin { get { return gddMin; } }
        public Landis.Library.Parameters.Species.AuxParm<int> GDDmax { get { return gddMax; } }
        public Landis.Library.Parameters.Species.AuxParm<int> MinJanTemp { get { return minJanTemp; } }
        public Landis.Library.Parameters.Species.AuxParm<double> MaxDrought { get { return maxDrought; } }
        public Landis.Library.Parameters.Species.AuxParm<double> LeafLongevity { get { return leafLongevity; } }
        //---------------------------------------------------------------------
        /// <summary>
        /// Can the species resprout epicormically following a fire?
        /// </summary>
        public Landis.Library.Parameters.Species.AuxParm<bool> Epicormic
        {
            get
            {
                return epicormic;
            }
            set
            {
                epicormic = value;
            }
        }

        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<double> LeafLignin
        {
            get
            {
                return leafLignin;
            }
        }
        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<double> WoodLignin
        {
            get
            {
                return woodLignin;
            }
        }
        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<double> CoarseRootLignin
        {
            get
            {
                return coarseRootLignin;
            }
        }
        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<double> FineRootLignin
        {
            get
            {
                return fineRootLignin;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> LeafCN
        {
            get
            {
                return leafCN;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> WoodCN
        {
            get
            {
                return woodCN;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> CoarseRootCN
        {
            get
            {
                return coarseRootCN;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> FoliageLitterCN
        {
            get
            {
                return foliageLitterCN;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> FineRootCN
        {
            get
            {
                return fineRootCN;
            }
        }
        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<int> RootingDepth
        {
            get
            {
                return rootingDepth;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<int> MaxANPP
        {
            get
            {
                return maxANPP;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<int> MaxBiomass
        {
            get
            {
                return maxBiomass;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Definitions of sufficient light probabilities.
        /// </summary>
        public List<ISufficientLight> LightClassProbabilities
        {
            get
            {
                return sufficientLight;
            }
            set
            {
                Debug.Assert(sufficientLight.Count != 0);
                sufficientLight = value;
            }
        }
        //---------------------------------------------------------------------
        public double Latitude
        {
            get
            {
                return latitude;
            }
        }

        public double InitialFineFuels { get { return initFineFuels; } }
        public double InitialMineralN { get { return initMineralN; } }
        public double InitialMicrobialC { get { return initMicrobialC; } }
        public double InitialMicrobialN { get { return initMicrobialN; } }
        public double InitialEnzymeConc { get { return initEnzymeConc; } }
        public double ActEnergySOMDepoly { get { return aes_som_depoly; } }
        public double ActEnergyDOCUptake { get { return ae_doc_uptake; } }
        public double ExpConstSOMDepoly { get { return ec_som_depoly; } }
        public double ExpConstDOCUptake { get { return ec_uptake; } }
        public double FractionSOMUnprotect { get { return frac_som_unprotect; } }
        public double CNEnzymes { get { return cn_enzymes; } }
        public double KmSOMDepoly { get { return km_dep; } }
        public double KmDOCUptake { get { return km_upt; } }
        public double EnzTurnRate { get { return r_death; } }
        public double MicrobialTurnRate { get { return r_ecloss; } }
        public double CarbonUseEfficiency { get { return c_use_efficiency; } }
        public double PropEnzymeSOM { get { return p_enz_SOC; } }
        public double PropCEnzymeProduction { get { return pconst; } }
        public double PropNEnzymeProduction { get { return qconst; } }
        public double FractDeadMicrobialBiomassSOM { get { return mic_to_som; } }
        public double MMConstantO2 { get { return km_o2; } }
        public double DiffConstantO2 { get { return dgas; } }
        public double DiffConstantSOMLiquid { get { return dliq; } }
        public double FractionVolumeO2 { get { return o2airfrac; } }
        public double InitialDOCFraction { get { return initDOCFraction; } }
        public double InitialDONFraction { get { return initDONFraction; } }
        public double FractionLitterDecayToDOC { get { return fractionLitterDecayToDOC; } }
        public double SoilMoistureA { get { return soilmoistA; } }
        public double SoilMoistureB { get { return soilmoistB; } }
        public double DenitrificationRate { get { return denitrif; } }

        //---------------------------------------------------------------------

        //public string AgeOnlyDisturbanceParms
        //{
        //    get {
        //        return ageOnlyDisturbanceParms;
        //    }
        //    set {
        //        string path = value;
        //        if (path.Trim(null).Length == 0)
        //            throw new InputValueException(path,"\"{0}\" is not a valid path.",path);
        //        ageOnlyDisturbanceParms = value;
        //    }
        //}

        //---------------------------------------------------------------------
        public string SoilDepthMapName
        {
            get
            {
                return soilDepthMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilDepthMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilDrainMapName
        {
            get
            {
                return soilDrainMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilDrainMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilBaseFlowMapName
        {
            get
            {
                return soilBaseFlowMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilBaseFlowMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilStormFlowMapName
        {
            get
            {
                return soilStormFlowMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilStormFlowMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilFieldCapacityMapName
        {
            get
            {
                return soilFieldCapacityMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilFieldCapacityMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilWiltingPointMapName
        {
            get
            {
                return soilWiltingPointMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilWiltingPointMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilPercentSandMapName
        {
            get
            {
                return soilPercentSandMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilPercentSandMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilPercentClayMapName
        {
            get
            {
                return soilPercentClayMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilPercentClayMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilBulkDensityMapName
        {
            get
            {
                return soilBulkDensityMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilBulkDensityMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilParticleDensityMapName
        {
            get
            {
                return soilParticleDensityMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilParticleDensityMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string InitialSOC_PrimaryMapName
        {
            get
            {
                return initialSOC_PrimaryMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialSOC_PrimaryMapName = value;

            }
        }

        //---------------------------------------------------------------------

        public string InitialSON_PrimaryMapName
        {
            get
            {
                return initialSON_PrimaryMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialSON_PrimaryMapName = value;
            }
        }

        //---------------------------------------------------------------------

        public string InitialDeadSurfaceMapName
        {
            get
            {
                return initialDeadSurfaceMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialDeadSurfaceMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string InitialDeadSoilMapName
        {
            get
            {
                return initialDeadSoilMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialDeadSoilMapName = value;
            }
        }
        //---------------------------------------------------------------------

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
            maximumShadeLAI[shadeClass] = newValue;
            //minRelativeBiomass[shadeClass][ecoregion] = newValue;
        }
        //---------------------------------------------------------------------

        public void SetFunctionalType(ISpecies species,
                                     InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            sppFunctionalType[species] = CheckBiomassParm(newValue, 0, 100);
        }
        //---------------------------------------------------------------------

        //public void SetNFixer(ISpecies           species,
        //                             InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    nTolerance[species] = CheckBiomassParm(newValue, 1, 4);
        //}

        //---------------------------------------------------------------------

        public void SetGDDmin(ISpecies species,
                                     InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            gddMin[species] = CheckBiomassParm(newValue, 1, 4000);
        }
        //---------------------------------------------------------------------

        public void SetGDDmax(ISpecies species,
                                     InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            gddMax[species] = CheckBiomassParm(newValue, 500, 7000);
        }
        //---------------------------------------------------------------------

        public void SetMinJanTemp(ISpecies species,
                                     InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            minJanTemp[species] = CheckBiomassParm(newValue, -60, 20);
        }
        //---------------------------------------------------------------------

        public void SetMaxDrought(ISpecies species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            maxDrought[species] = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetLeafLongevity(ISpecies species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            leafLongevity[species] = CheckBiomassParm(newValue, 1.0, 10.0);
        }

        //---------------------------------------------------------------------

        public void SetLeafLignin(ISpecies species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            leafLignin[species] = CheckBiomassParm(newValue, 0.0, 0.4);
        }
        //---------------------------------------------------------------------

        public void SetWoodLignin(ISpecies species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            woodLignin[species] = CheckBiomassParm(newValue, 0.0, 0.4);
        }
        //---------------------------------------------------------------------

        public void SetCoarseRootLignin(ISpecies species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            coarseRootLignin[species] = CheckBiomassParm(newValue, 0.0, 0.4);
        }
        //---------------------------------------------------------------------

        public void SetFineRootLignin(ISpecies species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            fineRootLignin[species] = CheckBiomassParm(newValue, 0.0, 0.4);
        }
        //---------------------------------------------------------------------

        public void SetLeafCN(ISpecies species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            leafCN[species] = CheckBiomassParm(newValue, 5.0, 100.0);
        }
        //---------------------------------------------------------------------

        public void SetWoodCN(ISpecies species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            //woodCN[species] = CheckBiomassParm(newValue, 5.0, 600.0);
            woodCN[species] = CheckBiomassParm(newValue, 5.0, 900.0);
        }
        //---------------------------------------------------------------------

        public void SetCoarseRootCN(ISpecies species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            coarseRootCN[species] = CheckBiomassParm(newValue, 5.0, 500.0);
        }
        //---------------------------------------------------------------------

        public void SetFoliageLitterCN(ISpecies species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            foliageLitterCN[species] = CheckBiomassParm(newValue, 5.0, 100.0);
        }
        //---------------------------------------------------------------------

        public void SetFineRootCN(ISpecies species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            fineRootCN[species] = CheckBiomassParm(newValue, 5.0, 100.0);
        }
        //---------------------------------------------------------------------
        public void SetRootingDepth(ISpecies species,
                                         InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            rootingDepth[species] = CheckBiomassParm(newValue, 0, 5000);
        }
        //---------------------------------------------------------------------

        public void SetMaxANPP(ISpecies species,
                                          InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            maxANPP[species] = CheckBiomassParm(newValue, 2, 1000);
        }
        //---------------------------------------------------------------------

        public void SetMaxBiomass(ISpecies species, InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            maxBiomass[species] = CheckBiomassParm(newValue, 2, 100000);
        }
        //---------------------------------------------------------------------

        public void SetAtmosNslope(InputValue<double> newValue)
        {
            atmosNslope = CheckBiomassParm(newValue, -1.0, 2.0);
        }
        //---------------------------------------------------------------------
        public void SetAtmosNintercept(InputValue<double> newValue)
        {
            atmosNintercept = CheckBiomassParm(newValue, -1.0, 2.0);
        }
        //---------------------------------------------------------------------
        public void SetLatitude(InputValue<double> newValue)
        {
            //Debug.Assert(ecoregion != null);
            latitude = CheckBiomassParm(newValue, 0.0, 80.0);
        }
       
        //---------------------------------------------------------------------
        public void SetInitFineFuels(InputValue<double> newValue)
        {
            initFineFuels = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------
        public void SetInitMineralN(InputValue<double> newValue)
        {
            initMineralN = CheckBiomassParm(newValue, 0.0, 50.0);
        }

        //---------------------------------------------------------------------
        public void SetInitMicrobialC(InputValue<double> newValue)
        {
            initMicrobialC = CheckBiomassParm(newValue, 0.0, 5.0);
        }

        //---------------------------------------------------------------------
        public void SetInitMicrobialN(InputValue<double> newValue)
        {
            initMicrobialN = CheckBiomassParm(newValue, 0.0, 5.0);
        }

        //---------------------------------------------------------------------
        public void SetInitEnzymeConc(InputValue<double> newValue)
        {
            initEnzymeConc = CheckBiomassParm(newValue, 0.0, 5.0);
        }

        //---------------------------------------------------------------------
        public void SetActEnergySOMDepoly(InputValue<double> newValue)
        {
            aes_som_depoly = CheckBiomassParm(newValue, 0.0, 200.0);
        }
        //---------------------------------------------------------------------
        public void SetActEnergyDOCUptake(InputValue<double> newValue)
        {
            ae_doc_uptake = CheckBiomassParm(newValue, 0.0, 200.0);
        }
        //---------------------------------------------------------------------
        public void SetECSOMDepoly(InputValue<double> newValue)
        {
            ec_som_depoly = CheckBiomassParm(newValue, 0.0, 200000000000);
        }
        //---------------------------------------------------------------------
        public void SetExpConstDOCUptake(InputValue<double> newValue)
        {
            ec_uptake = CheckBiomassParm(newValue, 0.0, 200000000000.0);
        }
        //---------------------------------------------------------------------
        public void SetFractionSOMUnprotect(InputValue<double> newValue)
        {
            frac_som_unprotect = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        public void SetCNEnzymes(InputValue<double> newValue)
        {
            cn_enzymes = CheckBiomassParm(newValue, 0.0, 5.0);
        }
        //---------------------------------------------------------------------
        public void SetKmSOMDepoly(InputValue<double> newValue)
        {
            km_dep = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetKmDOCUptake(InputValue<double> newValue)
        {
            km_upt = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        //---------------------------------------------------------------------

        public void SetEnzymeTurnRate(InputValue<double> newValue)
        {
            r_ecloss = CheckBiomassParm(newValue, 0.0, 0.1);
        }
        //---------------------------------------------------------------------

        public void SetMicrobialTurnRate(InputValue<double> newValue)
        {
            r_death = CheckBiomassParm(newValue, 0.0, 0.1);
        }
        //---------------------------------------------------------------------

        public void SetCarbonUseEfficiency(InputValue<double> newValue)
        {
            c_use_efficiency = CheckBiomassParm(newValue, 0.0, 0.9);
        }
        //---------------------------------------------------------------------

        public void SetPropEnzymeSOM(InputValue<double> newValue)
        {
            p_enz_SOC = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetPropCEnzymeProduction(InputValue<double> newValue)
        {
            pconst = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetPropNEnzymeProduction(InputValue<double> newValue)
        {
            qconst = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------
        public void SetFractDeadMicrobialBiomassSOM(InputValue<double> newValue)
        {
            mic_to_som = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------
        public void SetMMConstantO2(InputValue<double> newValue)
        {
            km_o2 = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------
        public void SetDiffConstantO2(InputValue<double> newValue)
        {
            dgas = CheckBiomassParm(newValue, 0.0, 5.0);
        }
        //---------------------------------------------------------------------

        public void SetDiffConstantSOMLiquid(InputValue<double> newValue)
        {
            dliq = CheckBiomassParm(newValue, 0.0, 5.0);
        }

        //---------------------------------------------------------------------
        public void SetFractionVolumeO2(InputValue<double> newValue)
        {
            o2airfrac = CheckBiomassParm(newValue, 0.0, 1.0);
        }


        //---------------------------------------------------------------------
        public void SetInitDOCFraction(InputValue<double> newValue)
        {
            initDOCFraction = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        public void SetInitDONFraction(InputValue<double> newValue)
        {
            initDONFraction = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        //---------------------------------------------------------------------
        public void SetFractionDOC(InputValue<double> newValue)
        {
            fractionLitterDecayToDOC = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        //---------------------------------------------------------------------
        public void SetSoilMoistureA (InputValue<double> newValue)
        {
            soilmoistA = CheckBiomassParm(newValue, 0.0, 50.0);
        }

        //---------------------------------------------------------------------
        public void SetSoilMoistureB (InputValue<double> newValue)
        {
            soilmoistA = CheckBiomassParm(newValue, 0.0, 50.0);
        }

        //----------------------------------
        public void SetDenitrif(InputValue<double> newValue)
        {
            denitrif = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public InputParameters(ISpeciesDataset speciesDataset, int litterCnt, int functionalCnt)
        {
            this.speciesDataset = speciesDataset;

            functionalTypes = new FunctionalTypeTable(functionalCnt);
            fireReductionsTable = new FireReductions[11];
            harvestReductionsTable = new List<HarvestReductions>();

            sppFunctionalType = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            nFixer = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            adventRoots = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            gddMin = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            gddMax = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            minJanTemp = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            maxDrought = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            leafLongevity = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            epicormic = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            leafLignin = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            woodLignin = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            coarseRootLignin = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            fineRootLignin = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            leafCN = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            woodCN = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            coarseRootCN = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            foliageLitterCN = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            fineRootCN = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            rootingDepth = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            maxANPP = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            maxBiomass = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);

            maximumShadeLAI = new double[6];

            //minRelativeBiomass = new Ecoregions.AuxParm<Percentage>[6];
            //for (byte shadeClass = 1; shadeClass <= 5; shadeClass++) {
            //    minRelativeBiomass[shadeClass] = new Ecoregions.AuxParm<Percentage>(ecoregionDataset);
            //}
            sufficientLight = new List<ISufficientLight>();

        }

        //---------------------------------------------------------------------

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


