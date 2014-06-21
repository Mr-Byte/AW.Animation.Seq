﻿// Copyright 2014 Joshua R. Rodgers
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
namespace Bloyteg.AW.Animation.Seq

module internal SeqLoader = 
    let readInt16 (stream : System.IO.Stream) : int = (stream.ReadByte() <<< 8) ||| (stream.ReadByte())
    let readInt32 (stream : System.IO.Stream) : int = 
        (stream.ReadByte() <<< 24) ||| (stream.ReadByte() <<< 16) ||| (stream.ReadByte() <<< 8) ||| (stream.ReadByte())
    
    let readString (stream : System.IO.Stream) length = 
        let stringBytes : byte array = Array.zeroCreate length
        stream.Read(stringBytes, 0, length) |> ignore
        System.Text.ASCIIEncoding.ASCII.GetString(stringBytes).TrimEnd(char 0s)
    
    let loadBinarySeqFromStream stream : Animation = 
        let frameCount = readInt16 stream
        let jointCount = readInt32 stream
        let modelNameLength = readInt16 stream
        let modelName = readString stream modelNameLength
        { FramesPerSecond = 30
          FrameCount = frameCount
          Joints = Seq.empty }
    
    let loadTextSeqFromStream stream : Animation = failwith "Not implemented."
    
    let (|Binary|Text|Unknown|) (stream : System.IO.Stream) = 
        match stream |> readInt32 with
        | (0x7F7F7F79) | (0x7F7F7F7A) -> Binary
        | (0x41575351) -> Text
        | _ -> Unknown
    
    let loadFromStream (stream : System.IO.Stream) = 
        match stream with
        | Binary -> loadBinarySeqFromStream stream
        | Text -> loadTextSeqFromStream stream
        | Unknown -> failwith "Unrecognized file type."

type Loader() = 
    member this.LoadFromStream(stream : System.IO.Stream) = SeqLoader.loadFromStream stream
