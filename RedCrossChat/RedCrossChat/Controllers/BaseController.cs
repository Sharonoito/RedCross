using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RedCrossChat.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace RedCrossChat
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Response for a success http call
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        protected JsonResult Success(string msg = null, object responseData = null)
        {
            return base.Json(new
            {
                success = true,
                message = (msg ?? "Success"),
                responseData = responseData
            });
        }

        /// <summary>
        /// Errors the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns></returns>
        protected JsonResult Error(string msg = null, string error = null, bool isWarning = false)
        {
            return base.Json(new
            {
                success = false,
                warning = isWarning,
                message = (msg ?? "Unknown Error"),
                error = (error ?? "Unknown Error")
            });
        }

        /// <summary>
        /// Result for Invalid model state
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="properties">The invalid properties</param>
        /// <returns></returns>
        protected JsonResult NotValid(string msg = null, object properties = null)
        {
            return base.Json(new
            {
                success = false,
                valid = false,
                message = (msg ?? "Unknown Error"),
                properties = properties
            });
        }




        public byte[] GetImage(string sBase64String)
        {
            byte[] bytes = null;
            if (!string.IsNullOrEmpty(sBase64String))
            {
                bytes = Convert.FromBase64String(sBase64String);
            }
            return bytes;
        }
    }
}
