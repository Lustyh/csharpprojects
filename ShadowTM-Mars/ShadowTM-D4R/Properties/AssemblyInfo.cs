using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("ShadowTM-Sa")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ShadowTM-Sa")]
[assembly: AssemblyCopyright("Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

//将 ComVisible 设置为 false 将使此程序集中的类型
//对 COM 组件不可见。  如果需要从 COM 访问此程序集中的类型，
//请将此类型的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("7cd28c62-2fc9-42a3-8825-e4ad4ba6cbe7")]

// 程序集的版本信息由下列四个值组成: 
//
//      主版本
//      次版本
//      生成号
//      修订号
//
//可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值，
// 方法是按如下所示使用“*”: :
// 3.0.0.3 add smt routing check
// 3.0.1.0 断网后会有一个死循环在get message, 将其跳出死循环
// 3.0.1.1 将shadow 加入firewall里面  有些电脑因防火墙原因导致不能正常通讯
//         tRunCmd("netsh", " advfirewall firewall add rule name=\"ShadowTM\" dir=in program=\"" + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + "\" action=allow")
// 3.0.1.2 remove socket while 循环
// 3.0.2.1 add new port for robot. 区分mars和robot的端口  mars(9090)  robot(8080)
// 3.0.2.2 记录SF query超时1s的log
// 3.0.2.3 更改第二个socket端口为 9091
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("3.0.2.3")]
[assembly: AssemblyFileVersion("3.0.2.3")]
