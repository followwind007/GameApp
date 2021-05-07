@echo off
cd proto
REM 生成二进制文件pb(首先删除在重新生成)

set pbPath=..\pb\

rd /s /q %pbPath%
md %pbPath%

set pb=.pb
for %%i in (*.*) do (
    echo encode %%i to pb
    ..\bin\protoc -o %pbPath%%%i%pb% %%i
)

echo pb create success
echo.

cd ..\pb
SetLocal EnableDelayedExpansion
REM 要查找的文件类型
set ext=*.pb
REM 重新命名文件
for /r %%a in (!ext!) do (
    REM 文件名
    set fn=%%~na
    REM 后缀
    set en=%%~xa
    REM 把字符串的最后4个字符赋值给变量hou
    set hou=!fn:~0,-6!
    echo !hou!!en!
    rename "%%a" "!hou!!en!"
)
echo pb rename success
echo.

cd ..\
REM 项目中pb文件存放目录
set destPath=..\Assets\StreamingAssets\pb

rd /s /q %destPath%
md %destPath%

echo copy to %destPath%
xcopy pb %destPath% /s /e
pause