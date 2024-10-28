using System;
using System.Collections.Generic;

namespace Web1.Models;

public partial class Student
{
    public int MaSv { get; set; }

    public string Date { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Group { get; set; } = null!;
}
