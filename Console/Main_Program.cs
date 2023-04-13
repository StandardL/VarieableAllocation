namespace Variable__allocation_algorithm;



internal class Main_Program
{
    static void Main(string[] args)
    {
        // 创建一个新的内存
        Memory memory = new Memory();

        int op;
        do
        {
            Assignment assignment;
            Console.Clear();
            menu();
            var input = Console.ReadLine();
            while (!int.TryParse(input, out op))
            {
                Console.WriteLine("请正确输入对应的操作数字！");
                input = Console.ReadLine();
            }
            switch (op)
            {
                case 1:
                    memory.Display();
                    break;
                case 2:
                    memory.Merge_Display();
                    break;
                case 3:
                    assignment = memory.Input();
                    memory.First_fit(assignment);
                    break;
                case 4:
                    assignment = memory.Input();
                    memory.Optimal_fit(assignment);
                    break;
                case 5:
                    assignment = memory.Input();
                    memory.Worst_fit(assignment);
                    break;
                case 6:
                    var _toDel = memory.ToDel();
                    memory.Detele(_toDel);
                    break;
                case 0:
                    break;
                default:
                    break;
            }
            Console.ReadLine();
        } while (op != 0);
        
    }

    // 操作菜单
    static void menu()
    {
        Console.WriteLine("————操作系统可变分区分配算法模拟程序————");
        Console.WriteLine("1.单独显示已分配分区表和空闲分区表");
        Console.WriteLine("2.合并显示分区表");
        Console.WriteLine("3.使用最先适应分配算法为一个新进程申请内存");
        Console.WriteLine("4.使用最优适应分配算法为一个新进程申请内存");
        Console.WriteLine("5.使用最坏适应分配算法为一个新进程申请内存");
        Console.WriteLine("6.结束一个进程");
        Console.WriteLine("0.退出程序");
    }
}
