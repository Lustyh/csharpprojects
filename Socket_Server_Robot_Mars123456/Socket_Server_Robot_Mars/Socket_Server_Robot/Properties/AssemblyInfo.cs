using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("Socket_Server_Robot")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Socket_Server_Robot")]
[assembly: AssemblyCopyright("Copyright ©  2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 会使此程序集中的类型
//对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型
//请将此类型的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("8630254e-93ec-44e1-a348-b7af50a5b0bf")]

// 程序集的版本信息由下列四个值组成: 
//
//      主版本
//      次版本
//      生成号
//      修订号
//
// 可以指定所有值，也可以使用以下所示的 "*" 预置版本号和修订号
// 方法是按如下所示使用“*”: :
// 2.0.1.0 添加Mars自动化通信协议
// 2.0.1.2 当收到ack(test_start)时，将status改为5
// 2.0.2.1 如果一次收到多条request,将其拆分
// 2.0.2.4 新增Mars回传的测试结果
// 2.0.2.9 修复Mars回传test result, central controller但是未改变result
// 2.0.3.0 B条码将B去掉
// 2.0.3.2 record SF data, 写log由using改成StreamWriter, 
// 2.0.3.4 记录status  重启程序显示之前的status
// 2.0.3.5 回到 2.0.3.2 版本
// 2.0.3.6 去掉start时去sf query failcount变量 failcount有 robot[0].Fail_count 得到  减少启动超时。记录SF query超时1s的log
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.3.9")]
[assembly: AssemblyFileVersion("2.0.3.9")]
