using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Landis.Library.Metadata;
using Landis.Core;
using Landis.Utilities;

namespace Landis.Extension.Succession.DGS
{
    public class MetadataHandler
    {
        
        public static ExtensionMetadata Extension {get; set;}

        public static void InitializeMetadata(int timestep, ICore mCore, 
            string SoilCarbonMapNames, 
            string SoilNitrogenMapNames, 
            string ANPPMapNames, 
            string ANEEMapNames, 
            string TotalCMapNames)
            //string LAIMapNames,
            //string ShadeClassMapNames)
        {
            ScenarioReplicationMetadata scenRep = new ScenarioReplicationMetadata() {
                RasterOutCellArea = PlugIn.ModelCore.CellArea,
                TimeMin = PlugIn.ModelCore.StartTime,
                TimeMax = PlugIn.ModelCore.EndTime,
            };

            Extension = new ExtensionMetadata(mCore){
                Name = "DGS-Succession",
                TimeInterval = timestep, 
                ScenarioReplicationMetadata = scenRep
            };

            //---------------------------------------
            //          table outputs:   
            //---------------------------------------

            Outputs.primaryLog = new MetadataTable<PrimaryLog>("DGS-succession-log.csv");
            Outputs.primaryLogShort = new MetadataTable<PrimaryLogShort>("DGS-succession-log-short.csv");
            Outputs.monthlyLog = new MetadataTable<MonthlyLog>("DGS-succession-monthly-log.csv");
            Outputs.reproductionLog = new MetadataTable<ReproductionLog>("DGS-reproduction-log.csv");
            Outputs.establishmentLog = new MetadataTable<EstablishmentLog>("DGS-prob-establish-log.csv");

            OutputMetadata tblOut_monthly = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "MonthlyLog",
                FilePath = Outputs.monthlyLog.FilePath,
                Visualize = true,
            };
            tblOut_monthly.RetriveFields(typeof(MonthlyLog));
            Extension.OutputMetadatas.Add(tblOut_monthly);

            OutputMetadata tblOut_primary = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "PrimaryLog",
                FilePath = Outputs.primaryLog.FilePath,
                Visualize = false,
            };
            tblOut_primary.RetriveFields(typeof(PrimaryLog));
            Extension.OutputMetadatas.Add(tblOut_primary);

            OutputMetadata tblOut_primaryShort = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "PrimaryLogShort",
                FilePath = Outputs.primaryLogShort.FilePath,
                Visualize = true,
            };
            tblOut_primaryShort.RetriveFields(typeof(PrimaryLogShort));
            Extension.OutputMetadatas.Add(tblOut_primaryShort);

            OutputMetadata tblOut_repro = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "ReproductionLog",
                FilePath = Outputs.reproductionLog.FilePath,
                Visualize = false,
            };
            tblOut_repro.RetriveFields(typeof(ReproductionLog));
            Extension.OutputMetadatas.Add(tblOut_repro);

            OutputMetadata tblOut_pest = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "EstablishmentLog",
                FilePath = Outputs.establishmentLog.FilePath,
                Visualize = false,
            };
            tblOut_repro.RetriveFields(typeof(EstablishmentLog));
            Extension.OutputMetadatas.Add(tblOut_pest);

            //---------------------------------------            
            //          map outputs:         
            //---------------------------------------
            if (ANPPMapNames != null)
            {
                PlugIn.ModelCore.UI.WriteLine("  ANPP Map Names = \"{0}\" ...", ANPPMapNames);
                string[] paths = { @"DGS", "AG_NPP-{timestep}.img" };
                OutputMetadata mapOut_ANPP = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Aboveground Net Primary Production",
                    FilePath = @"DGS\AG_NPP-{timestep}.img",  
                    //FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_ANPP);
            }

            if (ANEEMapNames != null)
            {
                string[] paths = { @"DGS", "NEE-{timestep}.img" };
                OutputMetadata mapOut_Nee = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Net Ecosystem Exchange",
                    FilePath = @"DGS\NEE-{timestep}.img",  
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_Nee);
            }
            if (SoilCarbonMapNames != null)
            {
                string[] paths = { @"DGS", "SOC-{timestep}.img" };
                OutputMetadata mapOut_SOC = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Soil Organic Carbon",
                    FilePath = @"DGS\SOC-{timestep}.img",  
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_SOC);
            }
            if (SoilNitrogenMapNames != null)
            {
                string[] paths = { @"DGS", "SON-{timestep}.img" };
                OutputMetadata mapOut_SON = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Soil Organic Nitrogen",
                    FilePath = @"DGS\SON-{timestep}.img",  
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_N_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_SON);
            }
            if (TotalCMapNames != null)
            {
                string[] paths = { @"DGS", "TotalC-{timestep}.img" };
                OutputMetadata mapOut_TotalC = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Total Carbon",
                    FilePath = @"DGS\TotalC-{timestep}.img",  
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_TotalC);
            }
//            if (LAImapnames != null) //These are new maps for testing and analysis purposes
//            {
//                OutputMetadata mapOut_LAI = new OutputMetadata()
//                {
//                    Type = OutputType.Map,
//                    Name = "LAI",
//                    FilePath = @"century\LAI-{timestep}.gis",  //century
//                    Map_DataType = MapDataType.Continuous,
//                   Map_Unit = FieldUnits.g_C_m2, //Not sure
//                    Visualize = true,
//                };
//                Extension.OutputMetadatas.Add(mapOut_LAI);
//            }
//            if (ShadeClassmapnames != null)
//            {
//                OutputMetadata mapOut_ShadeClass = new OutputMetadata()
//                {
//                    Type = OutputType.Map,
//                    Name = "ShadeClass",
//                    FilePath = @"century\ShadeClass-{timestep}.gis",  //century
//                    Map_DataType = MapDataType.Continuous,
//                   Map_Unit = FieldUnits.g_C_m2, //NOt sure
//                    Visualize = true,
//                };
//                Extension.OutputMetadatas.Add(mapOut_LAI);
//            }


            //---------------------------------------
            MetadataProvider mp = new MetadataProvider(Extension);
            mp.WriteMetadataToXMLFile("Metadata", Extension.Name, Extension.Name);




        }
    }
}
