using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarieableAllocation.Classes;
public class Memory
{
    private static int counter = 0;
    private static readonly int space = 128;  // CPU中的内存
    public int Space => space;

    private static bool[] table = new bool[space];  // 记录哪些空间是空余或被使用的,false是未使用的，true是已使用的
    private static List<Assignment> _assignments = new();  // 内存分配表
    private static List<Assignment> _freespace = new();   // 内存空余表
    public List<Assignment> Freespace
    {
        get
        {
            return _freespace;
        }
    }
    public List<Assignment> Assignments
    {
    get
        {
            return _assignments;
        } 
    }
    public double getPercent()
    {
        var T = 0;
        for (var i = 0; i < space; i++)
            if (table[i])
                T++;
        var per = T/(double)space;
        return per * 100;
    }
    public int getUsedMem()
    {
        var T = 0;
        for (var i = 0; i < space; i++)
            if (table[i])
                T++;
        return T;
    }
    public int getFreeMem()
    {
        var T = 0;
        for (var i = 0; i < space; i++)
            if (!table[i])
                T++;
        return T;
    }
    

    // 初始化内存
    public Memory()
    {
        if (counter == 0)
        {
            // 初始化已存在的作业
            _assignments.Add(new Assignment("SYSTEM", 0, 10));  //系统保留空间
            _assignments.Add(new Assignment("01", 30, 15));
            _assignments.Add(new Assignment("03", 45, 2));
            _assignments.Add(new Assignment("07", 50, 30));
            foreach (var assignment in _assignments)
            {
                for (var i = assignment.Start; i <= assignment.End; i++)
                {
                    table[i] = true;
                }
            }

            // 初始化空闲分区
            _freespace.Add(new Assignment("空闲区", 10, 20));
            _freespace.Add(new Assignment("空闲区", 47, 3));
            _freespace.Add(new Assignment("空闲区", 80, space - 80));
            foreach (var freespace in _freespace)
            {
                for (var i = freespace.Start; i <= freespace.End; i++)
                {
                    table[i] = false;
                }
            }
        }
        counter++;
    }
     ~Memory()  // 回收
    {
        counter--;
    }

    // 获取可用内存
    private int GetSystemReserved()
    {
        int reserved = 0;
        foreach (var assignment in _assignments)
        {
            reserved += assignment.Mem_len;
        }

        // 无法获取到系统的占用内存，默认没有占用
        return reserved;
    }

    // 对在内存中的进程进行排序
    private void SortAssignment()
    {
        // 对两个表各自进行排序
        _assignments = _assignments.OrderBy(t => t.Start).ToList();
        _freespace = _freespace.OrderBy(t => t.Start).ToList();
    }

    // 显示内存分配情况
    public void Display()
    {
        Debug.WriteLine("空闲分区表");
        Debug.WriteLine("空间分配作业\t起始地址\t结束地址\t地址长度");
        foreach (var freespace in _freespace)
        {
            Debug.WriteLine("{0, -10}\t{1, -10}\t{2, -10}\t{3, -10}", freespace.Name, freespace.Start, freespace.End, freespace.Mem_len);
        }
        Debug.WriteLine("");
        Debug.WriteLine("已分配分区表");
        Debug.WriteLine("空间分配作业\t起始地址\t结束地址\t地址长度");
        foreach (var assignment in _assignments)
        {
            Debug.WriteLine("{0, -10}\t{1, -10}\t{2, -10}\t{3, -10}", assignment.Name, assignment.Start, assignment.End, assignment.Mem_len);
        }
        Debug.WriteLine("");
    }
    // 合并成一个表输出
    public List<Assignment> Merge_Display()
    {


        List<Assignment> _memories = new List<Assignment>();

        int pos_a, pos_f;  // 两个移动指针
        pos_a = 0; pos_f = 0;
        while (pos_a < _assignments.Count && pos_f < _freespace.Count)
        {
            // 两个相互交叉插入
            if (_assignments[pos_a].Start <= _freespace[pos_f].Start)
            {
                _memories.Add(_assignments[pos_a]);
                pos_a++;
            }
            else
            {
                _memories.Add(_freespace[pos_f]);
                pos_f++;
            }
        }

        // 到结尾后，有任意一方没有做完的，把他插入
        if (pos_a < _assignments.Count)
        {
            while (pos_a < _assignments.Count)
            {
                _memories.Add(_assignments[pos_a]);
                pos_a++;
            }
        }
        if (pos_f < _freespace.Count)
        {
            while (pos_f < _freespace.Count)
            {
                _memories.Add(_freespace[pos_f]);
                pos_f++;
            }
        }

/*        Debug.WriteLine("内存分配表");
        Debug.WriteLine("空间分配作业\t起始地址\t结束地址\t地址长度");
        foreach (Assignment assignment in _memories)
        {
            Debug.WriteLine("{0, -10}\t{1, -10}\t{2, -10}\t{3, -10}", assignment.Name, assignment.Start, assignment.End, assignment.Mem_len);
        }*/

        return _memories;
    }

    public bool First_fit(Assignment assignment)
    {
        /*
         * TODO: 
         * 1.判断进程是否符合加入的条件
         * 2.分配空间
         * 3.修改已分配表
         * 4.修改空闲表
         * 5.修改已使用内存块
         * 6.最后还需要排序
         */
        // 获取占用的内存
        var system_reserved = GetSystemReserved();

        // 先判断系统可用于进程的内存是否足够大，令进程有机会运行
        if (assignment.Mem_len > space - system_reserved)
        {
            Debug.WriteLine("进程所需内存大于系统内存！该进程无法被运行，因此已退出！");
            return false;
        }

        // 获取当前待分配内存的进程的所需内存
        var minspace = assignment.Mem_len;

        Assignment? desination = null;
        // 到待分配的内存区域寻找第一个放得下这个进程的地方
        desination = _freespace.Find(t => t.Mem_len > minspace);

        if (desination == null)
        {
            // 为空说明没有足够的地方放下这个进程
            Debug.WriteLine("内存中没有足够的空间让进程{0}运行！进程已退出！", assignment.Name);
            return false;
        }

        // 新的作业进度到作业表中
        Assignment assignment1 = new();
        assignment1.Name = assignment.Name;
        assignment1.Start = desination.Start;
        assignment1.Mem_len = assignment.Mem_len;
        assignment1.End = assignment1.Start + assignment1.Mem_len - 1;
        _assignments.Add(assignment1);

        // 修改内存块
        for (var i = assignment1.Start; i <= assignment1.End; i++)
        {
            table[i] = true;
        }

        // 修改原空闲分区表中的数据
        var _start = assignment1.End + 1;
        var _memlen = desination.End - _start + 1;
        Assignment editspace = new(desination.Name, _start, _memlen);
        //desination = editspace;
        var pos = _freespace.FindIndex(t => t.Start == assignment1.Start);
        _freespace[pos] = editspace;

        // 记得排序
        SortAssignment();
        return true;
    }
    // 最优适应分配算法
    public bool Optimal_fit(Assignment assignment)
    {
        /*
         * TODO: 
         * 1.判断进程是否符合加入的条件
         * 2.分配空间
         * 3.修改已分配表
         * 4.修改空闲表
         * 5.修改已使用内存块
         * 6.最后还需要排序
         */
        // 获取占用的内存
        var system_reserved = GetSystemReserved();

        // 先判断系统可用于进程的内存是否足够大，令进程有机会运行
        if (assignment.Mem_len > space - system_reserved)
        {
            Debug.WriteLine("进程所需内存大于系统内存！该进程无法被运行，因此已退出！");
            return false;
        }
        // 获取当前待分配内存的进程的所需内存
        var minspace = assignment.Mem_len;
        // 先将剩余空间从小到大排序一下
        _freespace = _freespace.OrderBy(t => t.Mem_len).ToList();

        Assignment? desination = null;
        // 到待分配的内存区域寻找第一个放得下这个进程的地方
        desination = _freespace.Find(t => t.Mem_len > minspace);
        if (desination == null)
        {
            // 为空说明没有足够的地方放下这个进程
            Debug.WriteLine("内存中没有足够的空间让进程{0}运行！进程已退出！", assignment.Name);
            return false;
        }

        // 新的作业进度到作业表中
        Assignment assignment1 = new();
        assignment1.Name = assignment.Name;
        assignment1.Start = desination.Start;
        assignment1.Mem_len = assignment.Mem_len;
        assignment1.End = assignment1.Start + assignment1.Mem_len - 1;
        _assignments.Add(assignment1);

        // 修改内存块
        for (var i = assignment1.Start; i <= assignment1.End; i++)
        {
            table[i] = true;
        }

        // 修改原空闲分区表中的数据
        var _start = assignment1.End + 1;
        var _memlen = desination.End - _start + 1;
        Assignment editspace = new(desination.Name, _start, _memlen);
        var pos = _freespace.FindIndex(t => t.Start == assignment1.Start);
        _freespace[pos] = editspace;

        // 记得排序
        SortAssignment();
        return true;
    }
    // 最坏适应分配算法
    public bool Worst_fit(Assignment assignment)
    {
        /*
         * TODO: 
         * 1.判断进程是否符合加入的条件
         * 2.分配空间
         * 3.修改已分配表
         * 4.修改空闲表
         * 5.修改已使用内存块
         * 6.最后还需要排序
         */
        // 获取占用的内存
        var system_reserved = GetSystemReserved();

        // 先判断系统可用于进程的内存是否足够大，令进程有机会运行
        if (assignment.Mem_len > space - system_reserved)
        {
            Debug.WriteLine("进程所需内存大于系统内存！该进程无法被运行，因此已退出！");
            return false;
        }
        // 获取当前待分配内存的进程的所需内存
        var minspace = assignment.Mem_len;
        // 先将剩余空间大到小排序一下
        _freespace = _freespace.OrderByDescending(t => t.Mem_len).ToList();

        Assignment? desination = null;
        // 到待分配的内存区域寻找第一个放得下这个进程的地方
        desination = _freespace.Find(t => t.Mem_len > minspace);
        if (desination == null)
        {
            // 为空说明没有足够的地方放下这个进程
            Debug.WriteLine("内存中没有足够的空间让进程{0}运行！进程已退出！", assignment.Name);
            return false;
        }

        // 新的作业进度到作业表中
        Assignment assignment1 = new();
        assignment1.Name = assignment.Name;
        assignment1.Start = desination.Start;
        assignment1.Mem_len = assignment.Mem_len;
        assignment1.End = assignment1.Start + assignment1.Mem_len - 1;
        _assignments.Add(assignment1);

        // 修改内存块
        for (var i = assignment1.Start; i <= assignment1.End; i++)
        {
            table[i] = true;
        }

        // 修改原空闲分区表中的数据
        var _start = assignment1.End + 1;
        var _memlen = desination.End - _start + 1;
        Assignment editspace = new(desination.Name, _start, _memlen);
        var pos = _freespace.FindIndex(t => t.Start == assignment1.Start);
        _freespace[pos] = editspace;

        // 记得排序
        SortAssignment();
        return true;
    }

    public bool Detele(string name)
    {
        /*
         * TODO:
         * 1.查找进程名字是否存在
         * 2.存在的话，删除并归还空间
         * 3.检查是否要和上方空闲内存合并
         * 4.检查是否要和下方空闲内存合并
         * 5.排序
         */

        // 先找到需要删除的元素
        var _desinated = _assignments.Find(t => t.Name == name);

        if (_desinated == null || _desinated.Name == null || _desinated.Name == "SYSTEM")
        {
            Debug.WriteLine("进程{0}不存在！本次操作没有影响内存的分配情况！", name);
            return false;
        }

        // 假设系统内没有重复的进程名
        _assignments.Remove(_desinated);  // 先把已分配内存表中的内存删除
        for (var i = _desinated.Start; i <= _desinated.End; i++)
        {
            table[i] = false;  // 已分配标志撤销
        }
        // 输出删除的进行的信息
        Debug.WriteLine("进程{0}的内存已回收！成功回收{1}单位内存", _desinated.Name, _desinated.Mem_len);
        var up = _desinated.Start;  // 用于检查上方是否有未分配的
        var down = _desinated.End;  // 用于检查下方是否有未分配的

        // 检查上下双方是否需要合并
        if (up - 1 >= 0 && down + 1 < space && !table[up - 1] && !table[down + 1])
        {
            // 若需要合并，获取上下方空闲的分区及其位置
            var _freeUp = _freespace.Find(t => t.End == up - 1);
            var _freeUpIndex = _freespace.FindIndex(t => t.End == up - 1);
            var _freeDown = _freespace.Find(t => t.Start == down + 1);
            var _freeDownIndex = _freespace.FindIndex(t => t.Start == down + 1);

            // 修改区间长度
            _freeUp.End = _freeDown.End;
            _freeUp.Mem_len = _freeUp.End - _freeUp.Start + 1;

            // 合并修改
            _freespace[_freeUpIndex] = _freeUp;
            // 删除下方的多余分区
            _freespace.Remove(_freeDown);
        }
        // 检查上方是否需要合并
        else if (up - 1 >= 0 && !table[up - 1])
        {
            // 若需要合并，获取上方空闲的分区及其位置
            var _freeUp = _freespace.Find(t => t.End == up - 1);
            var _freeUpIndex = _freespace.FindIndex(t => t.End == up - 1);

            // 修改分区长度
            _freeUp.End = down;
            _freeUp.Mem_len = _freeUp.End - _freeUp.Start + 1;

            // 合并修改
            _freespace[_freeUpIndex] = _freeUp;
        }
        // 检查下方是否需要合并
        else if (down + 1 < space && !table[down + 1])
        {
            // 若需要合并，获取下方空闲分区及其位置
            var _freeDown = _freespace.Find(t => t.Start == down + 1);
            var _freeDownIndex = _freespace.FindIndex(t => t.Start == down + 1);

            // 修改分区长度
            _freeDown.Start = up;
            _freeDown.Mem_len = _freeDown.End - _freeDown.Start + 1;

            // 合并修改
            _freespace[_freeDownIndex] = _freeDown;
        }

        // 排序，确保两个表是有序的
        SortAssignment();
        return true;
    }
}
