//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.Climate;
using Landis.Library.Parameters;

using System.Collections.Generic;
using System;
using System.IO;

namespace Landis.Extension.Succession.DGS
{
    public class SpeciesData 
    {
        public static double AdventitiousLayerDepth = 0.20; // [m]

        //public static Landis.Library.Parameters.Species.AuxParm<int> FuncType;
        //public static Landis.Library.Parameters.Species.AuxParm<bool> NFixer;
        //public static Landis.Library.Parameters.Species.AuxParm<bool> AdventRoots;
        //public static Landis.Library.Parameters.Species.AuxParm<int> GDDmin;
        //public static Landis.Library.Parameters.Species.AuxParm<int> GDDmax;
        //public static Landis.Library.Parameters.Species.AuxParm<int> MinJanTemp;
        //public static Landis.Library.Parameters.Species.AuxParm<double> MaxDrought;
        //public static Landis.Library.Parameters.Species.AuxParm<double> LeafLongevity;
        //public static Landis.Library.Parameters.Species.AuxParm<bool> Epicormic;
        //public static Landis.Library.Parameters.Species.AuxParm<double> LeafLignin;
        //public static Landis.Library.Parameters.Species.AuxParm<double> WoodLignin;
        //public static Landis.Library.Parameters.Species.AuxParm<double> CoarseRootLignin;
        //public static Landis.Library.Parameters.Species.AuxParm<double> FineRootLignin;
        //public static Landis.Library.Parameters.Species.AuxParm<double> LeafCN;
        //public static Landis.Library.Parameters.Species.AuxParm<double> WoodCN;
        //public static Landis.Library.Parameters.Species.AuxParm<double> CoarseRootCN;
        //public static Landis.Library.Parameters.Species.AuxParm<double> LeafLitterCN;
        //public static Landis.Library.Parameters.Species.AuxParm<double> FineRootCN;
        //public static Landis.Library.Parameters.Species.AuxParm<int> RootingDepth;
        //public static Landis.Library.Parameters.Species.AuxParm<int> Max_ANPP;
        //public static Landis.Library.Parameters.Species.AuxParm<int> Max_Biomass;
        //public static Landis.Library.Parameters.Species.AuxParm<int> FireTolerance;
        //public static Landis.Library.Parameters.Species.AuxParm<int> ShadeTolerance;
        //public static Landis.Library.Parameters.Species.AuxParm<double> LightLAIShape;
        //public static Landis.Library.Parameters.Species.AuxParm<double> LightLAIScale;
        //public static Landis.Library.Parameters.Species.AuxParm<double> LightLAILocation;
        //public static Landis.Library.Parameters.Species.AuxParm<double> LightLAIAdjust;


        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            //FuncType            = parameters.SppFunctionalType;
            //NFixer              = parameters.NFixer;
            //AdventRoots         = parameters.AdventRoots;
            //GDDmin              = parameters.GDDmin;
            //GDDmax              = parameters.GDDmax;
            //MinJanTemp          = parameters.MinJanTemp;
            //MaxDrought          = parameters.MaxDrought;
            //LeafLongevity       = parameters.LeafLongevity;
            //Epicormic           = parameters.Epicormic;
            //LeafLignin          = parameters.LeafLignin;
            //WoodLignin          = parameters.WoodLignin;
            //CoarseRootLignin    = parameters.CoarseRootLignin;
            //FineRootLignin      = parameters.FineRootLignin;
            //LeafCN              = parameters.LeafCN;
            //WoodCN              = parameters.WoodCN;
            //CoarseRootCN        = parameters.CoarseRootCN;
            //LeafLitterCN        = parameters.FoliageLitterCN;
            //FineRootCN          = parameters.FineRootCN;
            //RootingDepth        = parameters.RootingDepth;
            //Max_ANPP            = parameters.MaxANPP;
            //Max_Biomass         = parameters.MaxBiomass;
            //FireTolerance       = parameters.FireTolerance;
            //ShadeTolerance      = parameters.ShadeTolerance;

            //LightLAIShape       = parameters.LightLAIShape;
            //LightLAIScale       = parameters.LightLAIScale;
            //LightLAILocation    = parameters.LightLAILocation;
            //LightLAIAdjust      = parameters.LightLAIAdjust;

            foreach (ISpecies spp in PlugIn.ModelCore.Species)
            {
                try
                {
                    double maxLAI = PlugIn.Parameters.MaxLAI[spp];
                    //double maxLAI = FunctionalType.Table[SpeciesData.FuncType[spp]].MaxLAI;
                    //PlugIn.ModelCore.UI.WriteLine("Spp={0}, FT={1}", spp.Name, SpeciesData.FuncType[spp]);

                }
                catch (Exception)
                {
                    string mesg = string.Format("Species or Functional Type Missing: {0}", spp.Name);
                    throw new System.ApplicationException(mesg);
                }
            }

        }
    }
}
