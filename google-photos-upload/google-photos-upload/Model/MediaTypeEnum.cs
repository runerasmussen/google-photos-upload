using System;
using System.Collections.Generic;
using System.Text;

namespace google_photos_upload.Model
{
    enum MediaType
    {
        /// <summary>
        /// The file type is not known and the user should be alerted.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The file can be ignored during upload
        /// </summary>
        Ignore = 1,
        /// <summary>
        /// Photo file
        /// </summary>
        Photo = 2,
        /// <summary>
        /// Movie file
        /// </summary>
        Movie = 3
    }
}
