using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Document = iTextSharp.text.Document;
using Image = iTextSharp.text.Image;
using System.Net.Http;

public class PdfGenerator
{
    public void GeneratePdf(string htmlContent, string headerImagePath, string footerText)
    {
        // Create a document
        Document document = new Document();
        // Define margins
        document.SetMargins(36, 36, 72, 56); // Left, Right, Top, Bottom

        // Create a PdfWriter
        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("output1.pdf", FileMode.Create));
        // Add header and footer
        var pageEvents = new PdfPageEvents(headerImagePath, footerText);
        writer.PageEvent = pageEvents;

        // Open the document
        document.Open();

        // Convert HTML content to iTextSharp elements
        var htmlElements = HTMLWorker.ParseToList(new StringReader(htmlContent), null);

        // Add HTML elements to the document
        foreach (var element in htmlElements)
        {
            document.Add(element as IElement);
        }

        // Set the total page count after adding content
        pageEvents.TotalPages = writer.PageNumber;

        // Close the document
        document.Close();
    }
}

public class PdfPageEvents : PdfPageEventHelper
{
    private string headerImagePath;
    private string footerText;
    PdfContentByte cb;
    public int TotalPages { get; set; }

    public PdfPageEvents(string headerImagePath, string footerText)
    {
        this.headerImagePath = headerImagePath;
        this.footerText = footerText;
    }

    public override void OnEndPage(PdfWriter writer, Document document)
    {
        cb=writer.DirectContent;
        // Header
        PdfPTable table = new PdfPTable(2); // 2 columns
        table.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
        table.LockedWidth = true;

        // Left column for text
        PdfPCell textCell = new PdfPCell(new Phrase("headerText\r\n SubText"));
        textCell.Border = 0;
        table.AddCell(textCell);

        Image headerImage = Image.GetInstance(headerImagePath);
        headerImage.ScaleToFit(document.PageSize.Width - document.LeftMargin - document.RightMargin, document.TopMargin - 10);
        headerImage.SetAbsolutePosition(400, document.PageSize.Height - document.TopMargin);

        PdfPCell imageCell = new PdfPCell(headerImage, true);
        imageCell.Border = 0;
        table.AddCell(imageCell);

        //headerImage.SetAbsolutePosition(document.RightMargin, document.PageSize.Height - document.TopMargin);
        table.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);

        //Move the pointer and draw line to separate header section from rest of page
        cb.MoveTo(40, document.PageSize.Height - 70);
        cb.LineTo(document.PageSize.Width - 40, document.PageSize.Height - 70);
        cb.Stroke();

        //Move the pointer and draw line to separate footer section from rest of page
        cb.MoveTo(40, document.PageSize.GetBottom(60));
        cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(60));
        cb.Stroke();

        // Footer
        Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        Chunk chunk = new Chunk(footerText, footerFont);
        Phrase footerPhrase = new Phrase(chunk);

        // Page number and total page count
        int pageNumber = writer.PageNumber;
        ColumnText.ShowTextAligned(writer.DirectContent,
            Element.ALIGN_CENTER,
            new Phrase($"Page {pageNumber} of {TotalPages}", footerFont),
            (document.Right + document.Left) / 2,
            document.Bottom - 20,
            0);
    }
}
class Program
{
    static void Main(string[] args)
    {
        string htmlContent = "{<header class='clearfix'><h1>INVOICE</h1><div id='company' class='clearfix'><div>Company Name</div><div>455 John Tower,<br /> AZ 85004, US</div><div>(602) 519-0450</div><div><a href='mailto:company@example.com'>company@example.com</a></div></div><div id='project'><div><span>PROJECT</span> Website development</div><div><span>CLIENT</span> John Doe</div><div><span>ADDRESS</span> 796 Silver Harbour, TX 79273, US</div><div><span>EMAIL</span> <a href='mailto:john@example.com'>john@example.com</a></div><div><span>DATE</span> April 13, 2016</div><div><span>DUE DATE</span> May 13, 2016</div></div></header><main><table><thead><tr><th class='service'>SERVICE</th><th class='desc'>DESCRIPTION</th><th>PRICE</th><th>QTY</th><th>TOTAL</th></tr></thead><tbody><tr><td class='service'>Design</td><td class='desc'>Creating a recognizable design solution based on the company's existing visual identity</td><td class='unit'>$400.00</td><td class='qty'>2</td><td class='total'>$800.00</td></tr><tr><td colspan='4'>SUBTOTAL</td><td class='total'>$800.00</td></tr><tr><td colspan='4'>TAX 25%</td><td class='total'>$200.00</td></tr><tr><td colspan='4' class='grand total'>GRAND TOTAL</td><td class='grand total'>$1,000.00</td></tr></tbody></table><div id='notices'><div>NOTICE:</div><div class='notice'>A finance charge of 1.5% will be made on unpaid balances after 30 days.</div></div></main><footer>Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.Invoice was created on a computer and is valid without the signature and seal.</footer>}";
        string headerImagePath = "D:\\Project\\PDFDynamic\\Image\\Extended Warrenty.png"; // Path to your header image
        string footerText = "Footer Text";

        PdfGenerator pdfGenerator = new PdfGenerator();
        pdfGenerator.GeneratePdf(htmlContent, headerImagePath, footerText);

        Console.WriteLine("PDF generated successfully!");
    }
}
