using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VarieableAllocation.Classes;
using VarieableAllocation.Dialogs;
using VarieableAllocation.ViewModels;

namespace VarieableAllocation.Views;

public sealed partial class MainPage : Page
{
    private readonly Memory _memory = new();
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();

        var value = _memory.getPercent();
        var text = value.ToString("F2");
        var totalmemory = _memory.Space.ToString();


        text = _memory.getUsedMem().ToString() + " / " + totalmemory + " 单位";
        UsedPercentText.Text = text;

        UsedProgressBar.Value = value;
        UsedProgressBar.IsIndeterminate = false;

        UsedSpaceText.Text = _memory.getUsedMem().ToString();
        FreeSpaceText.Text = _memory.getFreeMem().ToString();
        TotalSpaceText.Text = totalmemory;
    }

    private async void FirstFitButton_Click(object sender, RoutedEventArgs e)
    {
        var _name = AssignNameInput.Text;
        var _memlen = AssignMemInput.Text;
        int memlen;

        Debug.WriteLine("进入了FirstButtonClick函数！");
        if (_name == "" || _memlen == "" ||  !int.TryParse(_memlen, out memlen))
        {
            ErrorDialog2 errorDialog2 = new ErrorDialog2();
            errorDialog2.XamlRoot = this.XamlRoot;
            errorDialog2.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;

            var result = await errorDialog2.ShowAsync();

            MainInfoBar.Title = "错误";
            MainInfoBar.Severity = InfoBarSeverity.Error;
            MainInfoBar.Message = "请确保进程名不为空或进程所需内存仅为数字！";
        }
        else
        {
            Assignment assignment = new Assignment(_name, -1, memlen);

            if (!_memory.First_fit(assignment))  // 插入失败了
            {
                MainInfoBar.Title = "失败";
                MainInfoBar.Severity = InfoBarSeverity.Error;
                MainInfoBar.Message = "进程所需内存大于系统内存或内存中没有足够的空间让进程运行！该进程无法被运行，因此已退出！";
            }
            else
            {
                MainInfoBar.Title = "插入成功";
                MainInfoBar.Severity = InfoBarSeverity.Success;
                MainInfoBar.Message = "使用最先适应分配算法插入成功！";
                RefreshTip.IsOpen = true;
            }
        }
    }

    private async void OptimalFitButton_Click(object sender, RoutedEventArgs e)
    {
        var _name = AssignNameInput.Text;
        var _memlen = AssignMemInput.Text;
        int memlen;

        if (_name == "" || _memlen == "" || !int.TryParse(_memlen, out memlen))
        {
            ErrorDialog2 errorDialog2 = new ErrorDialog2();
            errorDialog2.XamlRoot = this.XamlRoot;
            errorDialog2.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;

            var result = await errorDialog2.ShowAsync();

            MainInfoBar.Title = "错误";
            MainInfoBar.Severity = InfoBarSeverity.Error;
            MainInfoBar.Message = "请确保进程名不为空或进程所需内存仅为数字！";
        }
        else
        {
            Assignment assignment = new Assignment(_name, -1, memlen);
            if (!_memory.Optimal_fit(assignment))
            {
                MainInfoBar.Title = "失败";
                MainInfoBar.Severity = InfoBarSeverity.Error;
                MainInfoBar.Message = "进程所需内存大于系统内存或内存中没有足够的空间让进程运行！该进程无法被运行，因此已退出！";
            }
            else
            {
                MainInfoBar.Title = "插入成功";
                MainInfoBar.Message = "使用最优秀适应分配算法插入成功！";
                MainInfoBar.Severity = InfoBarSeverity.Success;
                RefreshTip.IsOpen = true;
            }
        }
    }

    private async void WorstFitButton_Click(object sender, RoutedEventArgs e)
    {
        var _name = AssignNameInput.Text;
        var _memlen = AssignMemInput.Text;
        int memlen;

        if (_name == "" || _memlen == "" || !int.TryParse(_memlen, out memlen))
        {
            ErrorDialog2 errorDialog2 = new ErrorDialog2();
            errorDialog2.XamlRoot = this.XamlRoot;
            errorDialog2.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            var result = await errorDialog2.ShowAsync();

            MainInfoBar.Title = "错误";
            MainInfoBar.Severity = InfoBarSeverity.Error;
            MainInfoBar.Message = "请确保进程名不为空或进程所需内存仅为数字！";
        }
        else
        {
            Assignment assignment = new Assignment(_name, -1, memlen);
            if (!_memory.Worst_fit(assignment))
            {
                MainInfoBar.Title = "失败";
                MainInfoBar.Severity = InfoBarSeverity.Error;
                MainInfoBar.Message = "进程所需内存大于系统内存或内存中没有足够的空间让进程运行！该进程无法被运行，因此已退出！";
            }
            else
            {
                MainInfoBar.Title = "插入成功";
                MainInfoBar.Message = "使用最坏适应分配算法插入成功！";
                MainInfoBar.Severity = InfoBarSeverity.Success;
                RefreshTip.IsOpen = true;
            }
        }
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var _name = AssignNameInput.Text;

        if (_name == "")
        {
            ErrorDialog2 errorDialog2 = new ErrorDialog2();
            errorDialog2.XamlRoot = this.XamlRoot;
            errorDialog2.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            var result = await errorDialog2.ShowAsync();

            MainInfoBar.Title = "错误";
            MainInfoBar.Severity = InfoBarSeverity.Error;
            MainInfoBar.Message = "请确保进程名不为空！";
        }
        else
        {
            if (!_memory.Detele(_name))
            {
                MainInfoBar.Title = "删除失败";
                MainInfoBar.Severity = InfoBarSeverity.Error;
                MainInfoBar.Message = "进程不存在！本次操作没有影响内存的分配情况！";
            }
            else
            {
                MainInfoBar.Title = "删除成功";
                MainInfoBar.Severity = InfoBarSeverity.Warning;
                MainInfoBar.Message = "进程占用的内存已回收！请点击上方刷新的按钮刷新显示！";
            }
        }
    }

    private void ClearTextbox_Click(object sender, RoutedEventArgs e)
    {
        AssignNameInput.Text = null;
        AssignMemInput.Text = null;
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        var value = _memory.getPercent();
        var text = value.ToString("F2");
        var totalmemory = _memory.Space.ToString();


        text = _memory.getUsedMem().ToString() + " / " + totalmemory + " 单位";
        UsedPercentText.Text = text;

        UsedProgressBar.Value = value;
        UsedProgressBar.IsIndeterminate = false;

        UsedSpaceText.Text = _memory.getUsedMem().ToString();
        FreeSpaceText.Text = _memory.getFreeMem().ToString();
        TotalSpaceText.Text = totalmemory;
    }
}
