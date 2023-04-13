using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarieableAllocation.Classes;
public class Assignment
{
    public string? Name  // 进程名称
    {
        get; set;
    }
    public int Start  // 起始地址
    {
        get; set;
    }
    public int End  // 结束地址
    {
        get; set;
    }
    public int Mem_len  // 分区长度
    {
        get; set;
    }
    public Assignment(string name, int start, int used)
    {
        Name = name;
        Start = start;
        End = start + used - 1;
        Mem_len = used;
    }
    public Assignment()
    {
        Name = null;
        Start = 0;
        End = 0;
        Mem_len = 0;
    }
}
