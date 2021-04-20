#  **webrtc_remote_win** 

#### 介绍 
 #这是一个基于webrtc技术开发的远程工具，其功能类似于Teamviewer,向日葵等远程工具。

#### 软件架构
##### 技术框架是基于.net4.5.2的WPF,以下介绍三个启动项（请以管理员身份运行）:
###### 1.LYLRemote:这个启动项是软件的主程序
###### 2.WinServiceStartUp:这个启动项是windows服务，远程桌面为了完成一些系统级的操作，需要用windows服务启动
###### 3.StartApp：这个是安装包的启动程序，它通知windows服务，再由windows服务启动远程桌面。

#### 使用说明
1.  需要使用qq邮箱进行注册，一个账号登录于各台机器上，可以见到所有机器。(目前还存在较多的bug，请谅解)

#### 界面展示
 
1.  ![输入图片说明](http://m.qpic.cn/psc?/V50iXE9s345ckt2G1riZ1hYy2t1f3A5b/TmEUgtj9EK6.7V8ajmQrEKxPoWmt2jhtSNHZMkEHRoH.R6BVQxr2fODM*UVPkHyoSix5leS7DAjYi3CpfuqEvH10tKKqbNOji8VV1WQJjek!/b&bo=ZQe6AgAAAAADF.g!&rf=viewer_4&t=5 "1.jpg")
2.  ![输入图片说明](http://m.qpic.cn/psc?/V50iXE9s345ckt2G1riZ1hYy2t1f3A5b/TmEUgtj9EK6.7V8ajmQrEKw3NQxJ3dgZ0jX39yodNdzBH9LwpXQLShpPoXdGOhbC0KH3y*FnZqO02vbQW1xIwHJcFvvIMlwr2hFZIJUHt1w!/b&bo=eweOAgAAAAADF8I!&rf=viewer_4&t=5 "2.jpg")
3.  ![输入图片说明](http://m.qpic.cn/psc?/V50iXE9s345ckt2G1riZ1hYy2t1f3A5b/TmEUgtj9EK6.7V8ajmQrEObA.MvYlinMr6jyqxA0yQPU8F6P1ETQrDgVzzRSLPNsarc0YJJLLpSmMMDtb8SrZscytLe*.3mhNrNTW2*ke*8!/b&bo=dge9AgAAAAADF*w!&rf=viewer_4&t=5 "3.jpg")
 
 
