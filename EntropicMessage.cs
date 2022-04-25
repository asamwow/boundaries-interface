/* Boundaries - Interfaces and Models
 * Original Version Written by Samuel Jahnke 2022
 *
 * This library is free software: you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Library General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.  If not, see
 * <https://www.gnu.org/licenses/>.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace boundaries.Models {

   public class EntropicMessage : UnityMessage {

      public Entropic entropic;

      public byte chunkPosition { get; set; }

      public bool isTall;

      public EntropicMessage(Entropic entropic, int chunkX, int chunkY, int cellX, int cellY,
                             bool isTall)
          : base(chunkX, chunkY) {
         this.entropic = entropic;
         messageType = 9;
         this.isTall = isTall;
         SetChunkPosition((byte)cellX, (byte)cellY);
      }

      public EntropicMessage(string hex) : base(hex) {
         chunkPosition = (byte)Convert.ToInt32(hex.Substring(18, 2), 16);
         if (Convert.ToInt32(hex.Substring(20, 1), 2) == 0) {
            isTall = false;
         } else {
            isTall = true;
         }

         entropic = new Entropic(hex.Substring(21));
      }

      public override void Write(Stream stream) {
         base.Write(stream);
         string digit = chunkPosition.ToString("X2");
         for (int i = 0; i < 2; i++) {
            stream.WriteByte((byte)digit[i]);
         }
         byte boolByte = 0;
         if (isTall) {
            boolByte = 1;
         }
         digit = boolByte.ToString("X2");
         stream.WriteByte((byte)digit[1]);
         entropic.Write(stream);
      }

      [NotMapped]
      public byte x {
         get { return (byte)(chunkPosition % Chunk.ChunkSize); }
         set { SetChunkPosition(value, y); }
      }

      [NotMapped]
      public byte y {
         get { return (byte)(chunkPosition / Chunk.ChunkSize); }
         set { SetChunkPosition(x, value); }
      }

      public void SetChunkPosition(byte x, byte y) {
         if (x < 0 || x >= Chunk.ChunkSize || y < 0 || y >= Chunk.ChunkSize) {
            throw new Exception("Invalid Set Position index");
         }
         chunkPosition = (byte)(byte)(y * Chunk.ChunkSize + x);
      }
   }
}
