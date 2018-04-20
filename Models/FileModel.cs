using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace AspUploadSample.Models
{ 
    [Table("Files")]
    public class FileModel 
    {
        public FileModel()
        {
            var time = DateTime.Now;
            this.AddDate = time.ToString();
        }

        [Key]
        public int Id { get; set; }
        
        [Display(Name = "Add Date")]
        [HiddenInput(DisplayValue = true)]
        public string AddDate { get; set; }

        [Display(Name = "File Path")]
        [HiddenInput(DisplayValue = true)]
        [Index("FilePathIndex", IsUnique=true )]
        public string FilePath { get; set; }

        [Display(Name = "Upload Folder")]
        [HiddenInput(DisplayValue = true)]
        public string UploadFolder { get; set; }

        [Display(Name = "File Extension")]
        [HiddenInput(DisplayValue = true)]
        public string FileType { get; set; }

        [Display(Name = "File Name")]
        [Index("FileNameIndex", IsUnique = true)]
        public string FileName { get; set; }

        [Display(Name = "Last Editor")]
        public virtual string LastEditor { get; set; }

        [Display(Name = "Last Edit Date")]
        public virtual DateTime? LastEditDate { get; set; }

        [ForeignKey("User")]
        public string User_Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } 

        public virtual ApplicationUser User { get; set; }
    }
}