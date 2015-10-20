using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using WebUI.Infrastructure;
using Domain.Models;
using Domain.Abstract;
using Domain.Helpers;

namespace WebUI.Controllers
{
    [ApplicationAuthorizeAttribute, ApplicationExceptionFilterAttribute]
    [RoutePrefix("api/upload")]
    public class UploadController : ApiController
    {
     
        [Route("car")]
        [ApplicationAuthorizeAttribute]
        public async Task<IHttpActionResult> UploadCarImage()
        {
            // Check content size
            if (Request.Content.Headers.ContentLength > 2024000)
                return BadRequest(Resources.Language.ImageSizeError);


            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                return BadRequest(Resources.Language.ImageExtensionError);


            string root = HttpContext.Current.Server.MapPath("~/Content/uploads/img/cars");
            var provider = new FilenameMultipartFormDataStreamProvider(root);
            string path = "Content/uploads/img/cars/";

            // Read the form data.
            await Request.Content.ReadAsMultipartAsync(provider);

            // This illustrates how to get the file names.
            foreach (MultipartFileData file in provider.FileData)
            {
                if (ValidateMediaType(file.Headers.ContentType.MediaType)) BadRequest(Resources.Language.ImageExtensionError);
                Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                Trace.WriteLine("Server file path: " + file.LocalFileName);
                System.IO.FileInfo fi = new System.IO.FileInfo(file.LocalFileName);
                path += fi.Name;
            }

            return Json(new { filePath = path });
        }

        [Route("avatar")]
        [ApplicationAuthorizeAttribute]
        public async Task<IHttpActionResult> UploadUserAvatar()
        {
            // Check content size
            if (Request.Content.Headers.ContentLength > 2024000)
                return BadRequest(Resources.Language.ImageSizeError);


            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                BadRequest(Resources.Language.ImageExtensionError);


            string root = HttpContext.Current.Server.MapPath("~/Content/uploads/img/avatars");
            var provider = new FilenameMultipartFormDataStreamProvider(root);
            string path = "Content/uploads/img/avatars/";

            // Read the form data.
            await Request.Content.ReadAsMultipartAsync(provider);

            // This illustrates how to get the file names.
            foreach (MultipartFileData file in provider.FileData)
            {
                if (ValidateMediaType(file.Headers.ContentType.MediaType)) BadRequest(Resources.Language.ImageExtensionError);
                Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                Trace.WriteLine("Server file path: " + file.LocalFileName);
                System.IO.FileInfo fi = new System.IO.FileInfo(file.LocalFileName);
                path += fi.Name;
            }

            return Json(new { filePath = path });

        }

        private bool ValidateMediaType(string type)
        {
            if (type != "image/png" &&
                type != "image/jpeg" &&
                type != "image/jpg" &&
                type != "image/bmp") return true;
            return false;
        }
    }


    public class FilenameMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public FilenameMultipartFormDataStreamProvider(string path)
            : base(path)
        {
        }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : Guid.NewGuid().ToString();
            return name.Replace("\"", string.Empty);
        }
    }

}
