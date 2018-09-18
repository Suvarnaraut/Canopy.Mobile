﻿module Canopy.Mobile.Runner

open SampleTests
open System
open System.IO
open System.Net
open canopy.mobile
open Expecto

let downloadDemoApp () =
    let localFile = FileInfo("./temp/ApiDemos-debug.apk")
    if File.Exists localFile.FullName then
        printfn "app %s already exists" localFile.FullName
    else
        let appOnServer = "http://appium.github.io/appium/assets/ApiDemos-debug.apk"
        Directory.CreateDirectory localFile.Directory.FullName |> ignore
        use client = new WebClient()
        
        printfn "downloading %s to %s" appOnServer localFile.FullName
        client.DownloadFile(appOnServer, localFile.FullName)
        printfn "app downloaded"

    localFile.FullName

let startOnEmulator app =
    let settings = 
        { DefaultAndroidSettings with 
            AVDName = "Nexus_6_API_28"
            Silent = true }

    start settings app

[<EntryPoint>]
let main args =
    try
        try
            let app = downloadDemoApp()
            
            try
                startOnDevice app
            with 
            | _ -> startOnEmulator app

            runTestsWithArgs { defaultConfig with ``parallel`` = false } args tests
        with e ->
            printfn "Error: %s" e.Message
            -1
    finally
        quit()
