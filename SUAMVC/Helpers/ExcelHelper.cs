using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Spreadsheet.Row;
using DocumentFormat.OpenXml.Spreadsheet.Cell;
using LinqToExcel;
using LinqToExcel.Domain;
using SUAMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Helpers
{
    public class ExcelHelper
    {
        public List<PersonalExcelLayout> getPersonalDatos(String rutaDelArchivoExcel) {

            var book = new ExcelQueryFactory(rutaDelArchivoExcel);
            book.DatabaseEngine = DatabaseEngine.Ace;
            book.ReadOnly = true;
            var artistAlbums = from a in book.Worksheet("datos") select a;

            foreach (var a in artistAlbums)
            {
                string artistInfo = "Artist Name: {0}; Album: {1}";
                Console.WriteLine(string.Format(artistInfo, a["Nombre"], a["ApellidoMaterno"]));
            }
            var datos = (from row in book.Worksheet("datos")
                             let item = new PersonalExcelLayout
                             {
                                nombre = row["Nombre"].Cast<String>(),
                                apellidoMaterno = row["ApellidoMaterno"].Cast<String>(),
                                apellidoPaterno = row["ApellidoPaterno"].Cast<String>(),
                                edad = row["Edad"].Cast<int>()
   
                             }
                             select item).ToList();
            book.Dispose();
            return datos;
        }

        public void makeExcel(String excelName) {

            try{
            SpreadsheetDocument xl = SpreadsheetDocument.Create(excelName, SpreadsheetDocumentType.Workbook);

            WorkbookPart wbp = xl.AddWorkbookPart();
            WorksheetPart wsp = wbp.AddNewPart<WorksheetPart>();
            Workbook wb = new Workbook();
            FileVersion fv = new FileVersion();
            fv.ApplicationName = "Microsoft Office Excel";

            Worksheet ws = new Worksheet();
            WorkbookStylesPart wbsp = wbp.AddNewPart<WorkbookStylesPart>();
            // add styles to sheet
            wbsp.Stylesheet = CreateStylesheet();
            wbsp.Stylesheet.Save();

            // generate rows
            SheetData sd = CreateSheetData();
            ws.Append(sd);
            wsp.Worksheet = ws;
            wsp.Worksheet.Save();
            Sheets sheets = new Sheets();
            Sheet sheet = new Sheet();
            sheet.Name = "Sheet1";
            sheet.SheetId = 1;
            sheet.Id = wbp.GetIdOfPart(wsp);
            sheets.Append(sheet);
            wb.Append(fv);
            wb.Append(sheets);

            xl.WorkbookPart.Workbook = wb;
            xl.WorkbookPart.Workbook.Save();
            xl.Close();
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            
            }

        }
        private static SheetData CreateSheetData()
        {
            SheetData sheetData1 = new SheetData();
            DocumentFormat.OpenXml.Spreadsheet.Row row1 = new DocumentFormat.OpenXml.Spreadsheet.Row() { RowIndex = (UInt32Value)1U, Spans = new ListValue<StringValue>() { InnerText = "1:3" }, DyDescent = 0.25D };
            DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellReference = "A1", StyleIndex = (UInt32Value)1U };
 
            row1.Append(cell1);
 
            DocumentFormat.OpenXml.Spreadsheet.Row row2 = new DocumentFormat.OpenXml.Spreadsheet.Row() { RowIndex = (UInt32Value)2U, Spans = new ListValue<StringValue>() { InnerText = "1:3" }, DyDescent = 0.25D };
            DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellReference = "B2", StyleIndex = (UInt32Value)2U };
 
            row2.Append(cell2);
 
            DocumentFormat.OpenXml.Spreadsheet.Row row3 = new DocumentFormat.OpenXml.Spreadsheet.Row() { RowIndex = (UInt32Value)3U, Spans = new ListValue<StringValue>() { InnerText = "1:3" }, DyDescent = 0.25D };
            DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellReference = "C3", StyleIndex = (UInt32Value)3U };
 
            row3.Append(cell3);
 
            sheetData1.Append(row1);
            sheetData1.Append(row2);
            sheetData1.Append(row3);
 
            return sheetData1;
        }
 
        private static Stylesheet CreateStylesheet()
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
 
            Fonts fonts1 = new Fonts() { Count = (UInt32Value)1U, KnownFonts = true };
 
            Font font1 = new Font();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            Color color1 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };
 
            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);
 
            fonts1.Append(font1);
 
            Fills fills1 = new Fills() { Count = (UInt32Value)5U };
 
            // FillId = 0
            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };
            fill1.Append(patternFill1);
 
            // FillId = 1
            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };
            fill2.Append(patternFill2);
 
            // FillId = 2,RED
            Fill fill3 = new Fill();
            PatternFill patternFill3 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = "FFFF0000" };
            BackgroundColor backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill3.Append(foregroundColor1);
            patternFill3.Append(backgroundColor1);
            fill3.Append(patternFill3);
 
            // FillId = 3,BLUE
            Fill fill4 = new Fill();
            PatternFill patternFill4 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor2 = new ForegroundColor() { Rgb = "FF0070C0" };
            BackgroundColor backgroundColor2 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill4.Append(foregroundColor2);
            patternFill4.Append(backgroundColor2);
            fill4.Append(patternFill4);
 
            // FillId = 4,YELLO
            Fill fill5 = new Fill();
            PatternFill patternFill5 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor3 = new ForegroundColor() { Rgb = "FFFFFF00" };
            BackgroundColor backgroundColor3 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill5.Append(foregroundColor3);
            patternFill5.Append(backgroundColor3);
            fill5.Append(patternFill5);
 
            fills1.Append(fill1);
            fills1.Append(fill2);
            fills1.Append(fill3);
            fills1.Append(fill4);
            fills1.Append(fill5);
 
            Borders borders1 = new Borders() { Count = (UInt32Value)1U };
 
            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();
 
            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);
 
            borders1.Append(border1);
 
            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };
 
            cellStyleFormats1.Append(cellFormat1);
 
            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)4U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
            CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)4U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
 
            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);
            cellFormats1.Append(cellFormat5);
 
            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };
 
            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleMedium9" };
 
            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();
 
            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };
 
            stylesheetExtension1.Append(slicerStyles1);
 
            stylesheetExtensionList1.Append(stylesheetExtension1);
 
            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);
            return stylesheet1;
        }
    }
    }
}