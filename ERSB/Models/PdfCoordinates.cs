using GemBox.Pdf.Content;

namespace ERSB.Models
{
    public static class PdfCoordinates
    {
        public static PdfPoint Name { get; } = new PdfPoint(154.512, 689.4);
        public static PdfPoint RollNumber { get; } = new PdfPoint(429.696, 689.4);
        public static PdfPoint FatherName { get; } = new PdfPoint(154.512, 670.32);
        public static PdfPoint ErNo { get; } = new PdfPoint(429.696, 670.32);
        public static PdfPoint MotherName { get; } = new PdfPoint(154.512, 651.168);
        public static PdfPoint ResultDate { get; } = new PdfPoint(155.952, 212.976);
        //public static PdfPoint ResultDateAlt { get; } = new PdfPoint(155.95200000000003, 194.976);
        public static PdfPoint Result { get; } = new PdfPoint(100.440, 246.096);
        //public static PdfPoint ResultAlt { get; } = new PdfPoint(100.44000000000001, 230.68800000000005);
        //public static PdfPoint SGPA { get; } = new PdfPoint(475.704, 231.912);
        public static PdfPoint Sgpa { get; } = new PdfPoint(475.704, 262.29599999999994);
        //public static PdfPoint SGPAAlt { get; } = new PdfPoint(475.704, 246.88800000000003);
        public static PdfPoint SgpaBack { get; } = new PdfPoint(472.53599999999994, 262.29599999999994);
        public static PdfPoint Cgpa { get; } = new PdfPoint(544.896, 262.29599999999994);
        //public static PdfPoint CGPAAlt { get; } = new PdfPoint(544.6080000000001, 246.88800000000003);
    }
}
