using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models
{
    public class SummerNote
    {
        public SummerNote(string iDEditor, bool loadLibrary)
        {
            this.IDEditor = iDEditor;
            this.LoadLibrary = loadLibrary;
        }
        public string IDEditor { get; set; }
        public bool LoadLibrary { get; set; }

        public int height { get; set; } = 400;
        public string toolbar { get; set; } = @"
           [
             ['style', ['style']],
            ['font', ['bold', 'italic', 'underline', 'clear', 'strikethrough', 'superscript', 'subscript']],
            ['font', ['strikethrough', 'superscript', 'subscript']],
            ['fontsize', ['fontsize']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['height', ['height']],
            ['insert', ['link', 'picture', 'video', 'table', 'hr', 'elfinder']],
            ['view', ['fullscreen', 'codeview', 'help']],
            ['table', ['table']]
           ]
        ";
    }
}