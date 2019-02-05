using DIMSContainerDBEFDLL;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace UlaWebAgsWF
{
    /// <summary>
    /// Summary description for ImageDownloadHandler
    /// </summary>
    public class ImageDownloadHandler : IHttpHandler
    {
        DIMContainerDB_Revised_DevEntities dcde = new DIMContainerDB_Revised_DevEntities();

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                int CurrentTransID = Int32.Parse(context.Request.Headers["TransID"]);
                string PositionIDs = string.Empty;

                if (context.Request.Headers["PositionIDs"] != null)
                {
                    PositionIDs = context.Request.Headers["PositionIDs"].ToString();
                }

                DIMSContainerDBEFDLL.EntityProxies.ContainerTransactionProxy ct = (DIMSContainerDBEFDLL.EntityProxies.ContainerTransactionProxy)dcde.ContainerTransactions.Find(new ContainerTransaction { TransID = CurrentTransID });
                System.Web.HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.ClearContent();
                httpResponse.Clear();
                httpResponse.Headers.Clear();
                httpResponse.AddHeader("Content-Disposition", "attachment;filename=" + ct.TransID + ".zip");
                httpResponse.ContentType = "application/zip";

                if (!string.IsNullOrEmpty(PositionIDs) && Directory.GetFiles(ct.DIRLocation).Length > 0)
                {
                    using (var zipOutpuStream = new ZipOutputStream(httpResponse.OutputStream))
                    {
                        foreach (string pos in PositionIDs.Split(new char[] { ',' }))
                        {
                            foreach (string ContImg in Directory.GetFiles(ct.DIRLocation))
                            {
                                if (ContImg.Contains(pos + ".jpg"))
                                {
                                    byte[] ContImgBytes = File.ReadAllBytes(ContImg);
                                    var FileEntry = new ZipEntry(Path.GetFileName(ContImg))
                                    {
                                        Size = ContImgBytes.Length
                                    };

                                    zipOutpuStream.PutNextEntry(FileEntry);
                                    zipOutpuStream.Write(ContImgBytes, 0, ContImgBytes.Length);
                                }
                            }
                        }

                        zipOutpuStream.Flush();
                        zipOutpuStream.Close();
                    }

                    httpResponse.Headers.Add("Download", "Successful");
                }
                else
                {
                    httpResponse.Headers.Add("Download", "Unsuccessful");
                }
            }
            catch (Exception ex)
            {
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}