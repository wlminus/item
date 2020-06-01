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

				worksheet.Range["G1:I1"].Merge();

				worksheet.Range["G1"].Text = "TOAM Report";
				worksheet.Range["G1"].CellStyle.Font.Bold = true;
				worksheet.Range["G1"].CellStyle.Font.RGBColor = Color.FromArgb(42, 118, 189);
				worksheet.Range["G1"].CellStyle.Font.Size = 35;

				//Apply alignment in the cell G1
				worksheet.Range["G1"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
				worksheet.Range["G1"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;

				worksheet.Range["A7"].Text = "Ngày";
				worksheet.Range["B7"].Text = "Đồ dùng";
				worksheet.Range["C7"].Text = "Từ nhà";
				worksheet.Range["D7"].Text = "Từ phòng";
				worksheet.Range["E7"].Text = "Từ trạng thái";
				worksheet.Range["F7"].Text = "Đến nhà";
				worksheet.Range["G7"].Text = "Đến phòng";
				worksheet.Range["H7"].Text = "Đến trạng thái";
				worksheet.Range["I7"].Text = "Chi tiết";

				var listHistory = db.Transactions.ToList();
				List<HistoryVM> vmData = new List<HistoryVM>();
				var listItem1 = db.ItemInHouses.ToList();
				var listItem2 = db.ItemInRooms.ToList();
				var listStatus = db.ItemStatuses.ToList();
				var listHouse = db.Houses.ToList();
				var listRoom = db.Rooms.ToList();
				foreach (var trasn in listHistory)
				{
					HistoryVM tmp = new HistoryVM()
					{
						Date = trasn.Date,
						ItemId = trasn.ItemId,

						Item = listItem1.Where(i => i.Id == trasn.ItemId).SingleOrDefault() == null ? listItem2.Where(i => i.Id == trasn.ItemId).SingleOrDefault().Name : listItem1.Where(i => i.Id == trasn.ItemId).SingleOrDefault().Name,

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
				int current = 8;
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
				var cotcuoi = "A8:" + "I" + current.ToString();
				//Apply borders
				worksheet.Range[cotcuoi].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
				worksheet.Range[cotcuoi].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
				worksheet.Range[cotcuoi].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Grey_25_percent;
				worksheet.Range[cotcuoi].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Grey_25_percent;


				//Apply row height and column width to look good
				worksheet.Range["A1"].ColumnWidth = 22;
				worksheet.Range["B1:H1"].ColumnWidth = 15;
				worksheet.Range["i1"].ColumnWidth = 20;
				worksheet.Range["A1"].RowHeight = 47;
				worksheet.Range["A2"].RowHeight = 15;
				worksheet.Range["A3:A4"].RowHeight = 15;

				worksheet.Range["A7:I7"].CellStyle.Font.Color = ExcelKnownColors.White;
				worksheet.Range["A7:I7"].CellStyle.Font.Bold = true;
				worksheet.Range["A7:I7"].CellStyle.Color = Color.FromArgb(42, 118, 189);
				worksheet.Range["A7:I7"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;

				worksheet.Range["A7:I7"].RowHeight = 20;

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