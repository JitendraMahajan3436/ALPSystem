using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class Tracebilityview
    {
        public string Part_Id { get; set; }
        public string Part_Desc { get; set; }
        public string Part_NO_BOM { get; set; }
        public string Scan_Value { get; set; }
        public string Status { get; set; }
        public string VCode { get; set; }
        public string MFGDATE { get; set; }
        public string MFGSHIFT { get; set; }
        public string MFGSRNO { get; set; }
        public string PartNo { get; set; }
        public string Zpart { get; set; }
        public string Error { get; set; }
        public Tracebilityview(string Part_Id, string Part_Desc, string Part_NO_BOM, string Scan_Value, string Status, string VCode, string MFGDATE,
                                 string MFGSHIFT, string MFGSRNO, string PartNo, string Zpart, string Error)
        {
            this.Part_Id = Part_Id;
            this.Part_Desc = Part_Desc;
            this.Part_NO_BOM = Part_NO_BOM;
            this.Scan_Value = Scan_Value;
            this.Status = Status;
            this.VCode = VCode;
            this.MFGDATE = MFGDATE;
            this.MFGSHIFT = MFGSHIFT;
            this.MFGSRNO = MFGSRNO;
            this.PartNo = PartNo;
            this.Zpart = Zpart;
            this.Error = Error;

        }
    }

    public class TraceabilitySave
    {
        public string Message { get; set; }
        public bool Status { get; set; }

        public TraceabilitySave(string Message, bool Status)
        {
            this.Message = Message;
            this.Status = Status;
        }
    }


    public class PartListModel
    {
        public List<TracebilityPartList> obj { get; set; }

        public bool Linemode { get; set; }
        public bool Reworkmode { get; set; }
        public string EngineNo { get; set; }
        public string ModelCode { get; set; }
    }

    public class TracebilityPartList
    {
        public string Part_Id { get; set; }
        public string Part_Desc { get; set; }
        public string Part_NO_BOM { get; set; }
        public string Scan_Value { get; set; }
        public string Status { get; set; }
        public string VCode { get; set; }
        public string MFGDATE { get; set; }
        public string MFGSHIFT { get; set; }
        public string MFGSRNO { get; set; }
        public string PartNo { get; set; }
        public string Zpart { get; set; }
        public string Error { get; set; }

    }
}