@ECHO OFF

SET msbuildOld="%windir%\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
SET msbuild="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
SET slnPath="C:\Projects\powerpoint-integration\ARSnovaPPIntegration\source\ARSnovaPPIntegration.sln"

%msbuild% %slnPath% /p:VisualStudioVersion=14.0 /p:ToolsVersion=14.0 /p:DefineConstants="CODE_ANALYSIS;VSTO40"
