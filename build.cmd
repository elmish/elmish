@echo off
cls

.paket\paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)

.paket\paket.exe restore
if errorlevel 1 (
  exit /b %errorlevel%
)

call packages\Npm.js\tools\npm.cmd install

packages\FAKE\tools\FAKE.exe build.fsx %*
