# DebugConsole

[TOC]



## 远程调试

远程调试功能用于建立一条客户端到调试服务端的TCP连接，方便在移动平台上进行即时调试

### 服务端

服务端的主要功能是用于向客户端发送指令以及上传最新的脚本文件

![ServerMenu](\img\ServerMenu.png)

#### 创建服务器

打开场景DebugServer并且运行，当前连接的客户端列表将显示在Game视图左上角

#### 执行指令

选择Remote并点击Run即可在已经连接的远程客户端执行当前指令

![DebugConsoleRun](img\DebugConsoleRun.png)

#### 重载文件

Reload File会将当前选择的Lua脚本内容上传至客户端并且立刻重载，会替换所有实例的元表，但是并不会保存至本地，下次重新启动游戏，这些修改内容将不会被保存

#### 上传文件

Save File会将当前选择的Lua脚本上传至客户端，并保存至PersistDataPath，下次重新启动游戏即可使用刚刚上传的代码文件

#### 删除文件

Clear Selected会删除当前选择的已经上传至客户端的Lua脚本，Clear All会清空之前上传的所有脚本

### 客户端

客户端执行从服务端接收到的指令并且执行，把日志信息实时回传给服务器。长按设置面板右下角（或者在DebugConsole执行UIMgr:Open("DebugView")）调出Debug菜单，选择Debug选项

![ClientMenu](\img\ClientMenu.png)

#### 建立连接

输入服务器地址，点击Connect

#### 回传Log

勾选Auto Send Log会将客户端日志实时回传给服务器