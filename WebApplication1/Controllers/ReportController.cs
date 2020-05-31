using Microsoft.Owin.Security.Provider;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ReportController : Controller
    {
		ApplicationDbContext db;

		public ReportController()
		{
			db = new ApplicationDbContext();
		}

		public ActionResult Index()
        {
            return View();
        }

        public String CreateReport()
        {
			using (ExcelEngine excelEngine = new ExcelEngine())
			{
				IApplication application = excelEngine.Excel;

				application.DefaultVersion = ExcelVersion.Excel2016;

				//Create a workbook
				IWorkbook workbook = application.Workbooks.Create(1);
				IWorksheet worksheet = workbook.Worksheets[0];
				worksheet.IsGridLinesVisible = false;


				worksheet.Range["A3"].Text = "Cty TNHH TOAM";
				worksheet.Range["A4"].Text = "100 Dịch Vọng, Cầu Giấy";
				worksheet.Range["A5"].Text = "Phone: +84 912-123-456";

				worksheet.Range["A3:A5"].CellStyle.Font.Bold = true;

				worksheet.Range["D1:E1"].Merge();

				worksheet.Range["D1"].Text = "TOAM";
				worksheet.Range["D1"].CellStyle.Font.Bold = true;
				worksheet.Range["D1"].CellStyle.Font.RGBColor = Color.FromArgb(42, 118, 189);
				worksheet.Range["D1"].CellStyle.Font.Size = 35;

				//Apply alignment in the cell D1
				worksheet.Range["D1"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
				worksheet.Range["D1"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;

				worksheet.Range["D5:E5"].CellStyle.Color = Color.FromArgb(42, 118, 189);
				worksheet.Range["D7:E7"].CellStyle.Color = Color.FromArgb(42, 118, 189);

				//Apply known colors to the text in cells D5 to E8
				worksheet.Range["D5:E5"].CellStyle.Font.Color = ExcelKnownColors.White;
				worksheet.Range["D7:E7"].CellStyle.Font.Color = ExcelKnownColors.White;

				//Make the text as bold from D5 to E8
				worksheet.Range["D5:E8"].CellStyle.Font.Bold = true;

				//Apply alignment to the cells from D5 to E8
				worksheet.Range["D5:E8"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
				worksheet.Range["D5:E5"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
				worksheet.Range["D7:E7"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
				worksheet.Range["D6:E6"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
				//Apply alignment
				worksheet.Range["A7"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
				worksheet.Range["A7"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

				worksheet.Range["A9"].Text = "  Ngày";
				worksheet.Range["B9"].Text = "Đồ dùng";
				worksheet.Range["C9"].Text = "Từ nhà";
				worksheet.Range["D9"].Text = "Từ phòng";
				worksheet.Range["E9"].Text = "Từ trạng thái";
				worksheet.Range["F9"].Text = "Đến nhà";
				worksheet.Range["G9"].Text = "Đến phòng";
				worksheet.Range["H9"].Text = "Đến trạng thái";
				worksheet.Range["I9"].Text = "Chi tiết";

				var listHistory = db.Transactions.ToList();
				List<HistoryVM> vmData = new List<HistoryVM>();
				var listItem = db.ItemInHouses.ToList();
				var listStatus = db.ItemStatuses.ToList();
				var listHouse = db.Houses.ToList();
				var listRoom = db.Rooms.ToList();
				foreach (var trasn in listHistory)
				{
					HistoryVM tmp = new HistoryVM()
					{
						Date = trasn.Date,
						ItemId = trasn.ItemId,

						Item = listItem.Where(i => i.Id == trasn.ItemId).Single().Name,

						FromHouseId = trasn.FromHouseId,
						FromRoomId = trasn.FromRoomId,
						FromStatusId = trasn.FromStatusId,

						FromHouse = trasn.FromHouseId == 0 ? "KTD" : listHouse.Where(h => h.Id == trasn.FromHouseId).Single().Name,
						FromRoom = trasn.FromRoomId == 0 ? "KTD" : listRoom.Where(h => h.Id == trasn.FromRoomId).Single().Name,
						FromStatus = trasn.FromStatusId == 0 ? "KTD" : listStatus.Where(s => s.Id == trasn.FromStatusId).Single().Status,


						ToHouseId = trasn.ToHouseId,
						ToRoomId = trasn.ToRoomId,
						ToStatusId = trasn.ToStatusId,

						ToHouse = trasn.ToHouseId == 0 ? "KTD" : listHouse.Where(h => h.Id == trasn.ToHouseId).Single().Name,
						ToRoom = trasn.ToRoomId == 0 ? "KTD" : listRoom.Where(h => h.Id == trasn.ToRoomId).Single().Name,
						ToStatus = trasn.ToStatusId == 0 ? "KTD" : listStatus.Where(s => s.Id == trasn.ToStatusId).Single().Status,

						MediaId = trasn.MediaId,
						Media = trasn.Media,

						IsVerified = trasn.IsVerified,

						Description = trasn.Description
					};
					vmData.Add(tmp);
				}
				int current = 10;
				foreach(var trans in vmData)
				{
					var cot1 = "A" + current.ToString();
					System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
					dtDateTime = dtDateTime.AddSeconds(trans.Date).ToLocalTime();
					worksheet.Range[cot1].Text = dtDateTime.ToString();

					var cot2 = "B" + current.ToString();
					var cot3 = "C" + current.ToString();
					var cot4 = "D" + current.ToString();
					var cot5 = "E" + current.ToString();
					var cot6 = "F" + current.ToString();
					var cot7 = "G" + current.ToString();
					var cot8 = "H" + current.ToString();
					var cot9 = "I" + current.ToString();

					worksheet.Range[cot2].Text = trans.Item;

					worksheet.Range[cot3].Text = trans.FromHouse;
					worksheet.Range[cot4].Text = trans.FromRoom;
					worksheet.Range[cot5].Text = trans.FromStatus;

					worksheet.Range[cot6].Text = trans.ToHouse;
					worksheet.Range[cot7].Text = trans.ToRoom;
					worksheet.Range[cot8].Text = trans.ToStatus;

					worksheet.Range[cot9].Text = trans.Description;
					current++;
				}

				//Apply borders
				worksheet.Range["A10:E22"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
				worksheet.Range["A10:E22"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
				worksheet.Range["A10:E22"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Grey_25_percent;
				worksheet.Range["A10:E22"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Grey_25_percent;
				worksheet.Range["A23:E23"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
				worksheet.Range["A23:E23"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
				worksheet.Range["A23:E23"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
				worksheet.Range["A23:E23"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;

				//Apply font setting for cells with product details
				worksheet.Range["A3:E23"].CellStyle.Font.FontName = "Arial";
				worksheet.Range["A3:E23"].CellStyle.Font.Size = 10;
				worksheet.Range["A9:I9"].CellStyle.Font.Color = ExcelKnownColors.White;
				worksheet.Range["A9:I9"].CellStyle.Font.Bold = true;
				worksheet.Range["D23:E23"].CellStyle.Font.Bold = true;

				//Apply cell color
				worksheet.Range["A9:I9"].CellStyle.Color = Color.FromArgb(42, 118, 189);

				//Apply alignment to cells with product details
				worksheet.Range["A9"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
				worksheet.Range["C15:C22"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
				//worksheet.Range["D15:E15"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

				//Apply row height and column width to look good
				worksheet.Range["A1"].ColumnWidth = 36;
				worksheet.Range["B1"].ColumnWidth = 11;
				worksheet.Range["C1"].ColumnWidth = 8;
				worksheet.Range["D1:E1"].ColumnWidth = 18;
				worksheet.Range["A1"].RowHeight = 47;
				worksheet.Range["A2"].RowHeight = 15;
				worksheet.Range["A3:A4"].RowHeight = 15;
				worksheet.Range["A5"].RowHeight = 18;
				worksheet.Range["A6"].RowHeight = 29;
				worksheet.Range["A7"].RowHeight = 18;
				worksheet.Range["A8"].RowHeight = 15;
				//worksheet.Range["A9:A14"].RowHeight = 15;
				worksheet.Range["A9:I9"].RowHeight = 18;

				//Save the workbook to disk in xlsx format
				workbook.SaveAs("Report.xlsx", HttpContext.ApplicationInstance.Response, ExcelDownloadType.Open);
			}
			return "Done";
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}