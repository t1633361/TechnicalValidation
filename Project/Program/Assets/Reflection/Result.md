## 配置

CPU 9900k

内存 64G

Unity 2022.2.b1.2700

## 测试

每次测试执行10万次

直接赋值消耗时间： 0.0008468628      1万次的消耗在0.8毫秒

直接获取消耗时间：0.0008869171

反射赋值消耗时间：0.04090595

反射获取消耗时间：0.03493786

托管赋值消耗时间：0.01145744       1万次的消耗在114毫秒     1千次消耗在11毫秒    100次消耗在1毫秒

托管获取消耗时间：0.01170731 

EMIT赋值消耗时间：0.007076263     1万次的消耗在70毫秒

## 参考文档

[C# 反射性能优化](https://blog.csdn.net/xdedzl/article/details/109476593)

[C# 之 反射性能优化1](https://www.cnblogs.com/xinaixia/p/5777886.html)

[C#的反射为什么慢？怎么加快反射调用？](https://www.lfzxb.top/what-causes-csharp-invoke-method-by-reflection-slowly-and-how-to-solve-it/)





