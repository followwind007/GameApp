## 目录结构

Assets/Scripts/GameApp 存放了游戏中一些常用的模块

Assets/Scripts/GameApp/xxx/Documents 存放了该模块对应的文档

Assets/Scenes 存放了相关模块的示例

## GameApp

项目中的代码按照功能模块划分，各模块之间耦合度较低，使用时按需求集成对应模块即可

### AnimationRigging

路径：Scripts/GameApp/AnimationRigging

重写了Unity的Animation Rigging Package，做了一些简化，优化了自定义Constraint的流程

### AssetProcessor

路径：Scripts/GameApp/AssetProcessor

一个用于资源检查管线的工具，对于项目中的资产进行规范

### Assets

路径：Scripts/GameApp/Assets

Assets模块包含打包，资源构建，资源加载相关功能

### DataBinder

路径：Scripts/GameApp/DataBinder

提供Lua工具链中的数据绑定，生命周期同步功能

### DebugConsole

路径：Scripts/GameApp/DebugConsole

提供Lua工具链中的调试，数据展示，GM功能

### DynObject

路径：Scripts/GameApp/DynObject

一个类似于ProtoBuff的编解码协议，可在不修改C#代码的情况下加入新协议，支持lazy解析

### LogicGraph

路径：Scripts/GameApp/LogicGraph

可视化编程工具，目前支持Lua代码导出

### Timeline

路径：Scripts/GameApp/Timeline

提供一些Timeline的拓展轨道，以及一些实用的Util方法