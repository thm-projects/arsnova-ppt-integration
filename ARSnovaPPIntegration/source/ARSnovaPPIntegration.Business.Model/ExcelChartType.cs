using Excel = Microsoft.Office.Interop.Excel;

namespace ARSnovaPPIntegration.Business.Model
{
    public class ExcelChartType
    {
        public string Name { get; set; }

        public Excel.XlChartType ChartType { get; set; }
    }

}
