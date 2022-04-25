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
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace boundaries.Models {

   public class Appendage {

      public const byte CellSize = Cell.CellSize;

      // its planned to allow adding/removing appendages types in realtime
      public enum Type {
         Infantry,
         Mech,
         Recon,
         APC,
         Tank,
         MediumTank,
         Artillery,
         Rocket,
         AntiAir,
         Missiles,
         BCopter,
         TCopter,
         Fighter,
         Bomber,
         Lander,
         Cruiser,
         Submarine,
         BShip,
         HQ,
         Base,
         Harbour,
         Airport
      }

      public int id { get; set; }
      public Type type { get; set; }
      [JsonIgnore]
      public byte[] locations {
         get; set;
      }
      public byte entropy { get; set; }
      public int colorARGB { get; set; }
      public byte matterType { get; set; }
      public bool isBottomCell { get; set; }

      private Appendage(Type type, byte matterType) {
         this.type = type;
         this.locations = new byte[CellSize];
         this.matterType = matterType;
         isBottomCell = true;
      }

      public Appendage(string hex) {
         if (Convert.ToInt32(hex.Substring(0, 1), 2) == 0) {
            isBottomCell = false;
         } else {
            isBottomCell = true;
         }
         colorARGB = (int)Convert.ToInt32(hex.Substring(1, 8), 16);
         this.locations = new byte[CellSize];
         for (int i = 0; i < 8; i++) {
            string digit = hex.Substring(9 + i * 2, 2);
            locations[i] = (byte)Convert.ToInt32(digit, 16);
         }
      }

      public Appendage(Type type, byte x, byte y, Entropic entropic, byte matterType)
          : this(type, matterType) {
         entropic.appendages.Add(this);
         SetLocation(x, y);
      }

      public Appendage(Appendage other) {
         if (other != null) {
            Copy(other);
         }
      }

      public void Copy(Appendage other) {
         id = 0;
         type = other.type;
         locations = (byte[])other.locations.Clone();
         entropy = other.entropy;
         colorARGB = other.colorARGB;
         matterType = other.matterType;
         isBottomCell = other.isBottomCell;
      }

      public Appendage(Type type, int x, int y, Entropic entropic, int matterType)
          : this(type, (byte)x, (byte)y, entropic, (byte)matterType) {}

      public void SetLocation(int x, int y, bool isThere = true) {
         SetLocation((byte)x, (byte)y, isThere);
      }

      public void SetLocation(byte x, byte y, bool isThere = true) {
         if (x < 0 || x >= CellSize || y < 0 || y >= CellSize) {
            throw new Exception("Invalid Set Location index");
         }
         byte[] rowByte = new byte[1];
         rowByte[0] = locations[y];
         BitArray row = new BitArray(rowByte);
         row[x] = isThere;
         row.CopyTo(rowByte, 0);
         locations[y] = rowByte[0];
      }

      public bool GetLocation(byte x, byte y) {
         if (x < 0 || x >= CellSize || y < 0 || y >= CellSize) {
            throw new Exception("Invalid Get Location index");
         }
         byte[] rowByte = new byte[1];
         rowByte[0] = locations[y];
         BitArray row = new BitArray(rowByte);
         return row[x];
      }

      public bool GetLocation(int x, int y) { return GetLocation((byte)x, (byte)y); }

      public void Write(Stream stream) {
         byte boolByte = 0;
         if (isBottomCell) {
            boolByte = 1;
         }
         string digit = boolByte.ToString("X2");
         stream.WriteByte((byte)digit[1]);
         digit = colorARGB.ToString("X8");
         for (int i = 0; i < 8; i++) {
            stream.WriteByte((byte)digit[i]);
         }
         for (int i = 0; i < Chunk.ChunkSize; i++) {
            digit = locations[i].ToString("X2");
            stream.WriteByte((byte)digit[0]);
            stream.WriteByte((byte)digit[1]);
         }
      }
   }
}
