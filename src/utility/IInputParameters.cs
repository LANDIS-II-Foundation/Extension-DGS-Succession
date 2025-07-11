//  Author: Robert Scheller, Melissa Lucash

using Landis.Library.Succession;
using Landis.Utilities;
using Landis.Library.Parameters;
using System.Collections.Generic;

namespace Landis.Extension.Succession.DGS
{
    /// <summary>
    /// The parameters for biomass succession.
    /// </summary>
    public interface IInputParameters
    {
        int Timestep{ get;set;}
        SeedingAlgorithms SeedAlgorithm{ get;set;}
        string InitialCommunities{ get;set;}
        string InitialCommunitiesMap{ get;set;}
        string ClimateConfigFile { get; set; }
        string ShawGiplConfigFile { get; set; }
        string SoilDepthMapName { get; set; }
        string SoilDrainMapName { get; set; }
        string SoilBaseFlowMapName { get; set; }
        string SoilStormFlowMapName { get; set; }
        string SoilFieldCapacityMapName { get; set; }
        string SoilWiltingPointMapName { get; set; }
        string SoilPercentSandMapName { get; set; }
        string SoilPercentClayMapName { get; set; }
        string SoilBulkDensityMapName { get; set; }
        string SoilParticleDensityMapName { get; set; }
        string InitialSOC_PrimaryMapName { get; set; }
        string InitialSON_PrimaryMapName { get; set; }        
        string InitialDeadSurfaceMapName { get; set; }
        string InitialDeadSoilMapName { get; set; }       

        bool CalibrateMode { get; set; }
        WaterType WType {get;set;}
        double ProbEstablishAdjustment { get; set; }
        double[] MaximumShadeLAI { get; }
        bool SmokeModelOutputs { get; set; }

        ///// <summary>
        ///// A suite of parameters for species functional groups
        ///// </summary>
        //FunctionalTypeTable FunctionalTypes { get; set; }

        /// <summary>
        /// Parameters for fire effects on wood and leaf litter
        /// </summary>
        FireReductions[] FireReductionsTable { get; set; }

        /// <summary>
        /// Parameters for harvest or fuel treatment effects on wood and leaf litter
        /// </summary>
        List<HarvestReductions> HarvestReductionsTable { get; set; }

        /// <summary>
        /// Definitions of sufficient light probabilities.
        /// </summary>
        List<ISufficientLight> LightClassProbabilities { get; }

        Landis.Library.Parameters.Species.AuxParm<int> GDDmin { get; set; }
        Landis.Library.Parameters.Species.AuxParm<int> GDDmax { get; set; }
        Landis.Library.Parameters.Species.AuxParm<int> MinJanTemp { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> MaxDrought { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> LightLAIShape { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> LightLAIScale { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> LightLAILocation { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> LightLAIAdjust { get; set; }
        Landis.Library.Parameters.Species.AuxParm<bool> AdventRoots { get; set; }
        Landis.Library.Parameters.Species.AuxParm<bool> NFixer { get; set; }
        Landis.Library.Parameters.Species.AuxParm<bool> Epicormic { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> KLAI { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> MaxLAI { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> CompLimit { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> LeafBiomassToLAI { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> LeafLongevity { get; set; }
        Landis.Library.Parameters.Species.AuxParm<int> FoliageDropMonth { get; set; }
        Landis.Library.Parameters.Species.AuxParm<int> MaxANPP { get; set; }
        Landis.Library.Parameters.Species.AuxParm<int> MaxBiomass { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> TemperatureCurve1 { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> TemperatureCurve2 { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> TemperatureCurve3 { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> TemperatureCurve4 { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> MoistureCurve1 { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> MoistureCurve2 { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> MoistureCurve3 { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> MoistureCurve4 { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> FractionANPPtoLeaf { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> CoarseRootFraction { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> FineRootFraction { get; set; }
        Landis.Library.Parameters.Species.AuxParm<int> RootingDepth { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> LongevityMortalityShape { get; set; }
        Landis.Library.Parameters.Species.AuxParm<int> FireTolerance { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> LeafLignin { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> FineRootLignin { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> WoodLignin { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> CoarseRootLignin { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> LeafCN { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> FineRootCN { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> WoodCN { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> CoarseRootCN { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> FoliageLitterCN { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> WoodDecayRate { get; set; }
        Landis.Library.Parameters.Species.AuxParm<double> MonthlyWoodMortality { get; set; }
        
        //Landis.Library.Parameters.Species.AuxParm<int> SppFunctionalType{get;}
        //Landis.Library.Parameters.Species.AuxParm<bool> NFixer{get;}
        //Landis.Library.Parameters.Species.AuxParm<bool> AdventRoots { get; }
        //Landis.Library.Parameters.Species.AuxParm<int> GDDmin{get;}
        //Landis.Library.Parameters.Species.AuxParm<int> GDDmax{get;}
        //Landis.Library.Parameters.Species.AuxParm<int> MinJanTemp{get;}
        //Landis.Library.Parameters.Species.AuxParm<double> MaxDrought{get;}
        //Landis.Library.Parameters.Species.AuxParm<double> LeafLongevity {get;}
        //Landis.Library.Parameters.Species.AuxParm<bool> Epicormic {get;}
        //Landis.Library.Parameters.Species.AuxParm<double> LeafLignin {get;}
        //Landis.Library.Parameters.Species.AuxParm<double> WoodLignin {get;}
        //Landis.Library.Parameters.Species.AuxParm<double> CoarseRootLignin {get;}
        //Landis.Library.Parameters.Species.AuxParm<double> FineRootLignin {get;}
        //Landis.Library.Parameters.Species.AuxParm<double> LeafCN {get;}
        //Landis.Library.Parameters.Species.AuxParm<double> WoodCN {get;}
        //Landis.Library.Parameters.Species.AuxParm<double> CoarseRootCN {get;}
        //Landis.Library.Parameters.Species.AuxParm<double> FoliageLitterCN {get;}
        //Landis.Library.Parameters.Species.AuxParm<double> FineRootCN {get;}
        //Landis.Library.Parameters.Species.AuxParm<int> RootingDepth { get; }
        //Landis.Library.Parameters.Species.AuxParm<int> MaxANPP { get; }
        //Landis.Library.Parameters.Species.AuxParm<int> MaxBiomass { get; }
        //Landis.Library.Parameters.Species.AuxParm<int> FireTolerance { get; }
        //Landis.Library.Parameters.Species.AuxParm<int> ShadeTolerance { get; }
        //Landis.Library.Parameters.Species.AuxParm<double> LightLAIShape { get; }
        //Landis.Library.Parameters.Species.AuxParm<double> LightLAIScale { get; }
        //Landis.Library.Parameters.Species.AuxParm<double> LightLAILocation { get; }
        //Landis.Library.Parameters.Species.AuxParm<double> LightLAIAdjust { get; }


        double AtmosNslope {get;}
        double AtmosNintercept {get;}
        double Latitude { get; }
        double InitialFineFuels { get; }    
        double InitialMineralN { get; }
        double InitialMicrobialC { get; }
        double InitialMicrobialN { get; }
        double InitialEnzymeConc { get; }
        double ActEnergySOMDepoly { get; }
        double ActEnergyDOCUptake { get; }
        double ExpConstSOMDepoly { get; }
        double ExpConstDOCUptake { get; } 
        double FractionSOMUnprotect { get; }
        double CNEnzymes { get; }
        double KmSOMDepoly { get; }
        double KmDOCUptake { get; }
        double EnzTurnRate { get; }
        double MicrobialTurnRate { get; }
        double CarbonUseEfficiency { get; }
        double PropEnzymeSOM { get; }
        double PropCEnzymeProduction { get; }
        double PropNEnzymeProduction { get; }
        double FractDeadMicrobialBiomassSOM { get; }
        double MMConstantO2 { get; }
        double DiffConstantO2 { get; }
        double DiffConstantSOMLiquid { get; }
        double FractionVolumeO2 { get; }
        double InitialDOCFraction { get; }
        double InitialDONFraction { get; }
        double FractionLitterDecayToDOC { get; }
        //double SoilMoistureA { get; }
        //double SoilMoistureB { get; }
        double DenitrificationRate { get; }
    }
}
