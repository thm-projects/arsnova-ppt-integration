@ECHO OFF

SET msbuild="%windir%\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
SET slnPath="C:\Projects\powerpoint-integration\ARSnovaPPIntegration\source\ARSnovaPPIntegration.sln"

%msbuild% %slnPath%
if errorlevel 1 goto BuildFail
goto BuildSuccess

:BuildFail
echo.
echo *** BUILD FAILED ***
goto End

:BuildSuccess
echo.
echo **** BUILD SUCCESSFUL ***
goto end

:End
popd
