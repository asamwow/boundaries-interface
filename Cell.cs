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

   public class MissingCellNeighborException : Exception {}

   public class Cell {

      public const byte CellSize = 8;

      public const int MatterTypes = 16;

      public static int CellMatterCount {
         get { return CellSize * CellSize; }
      }

      public static Random rand = new Random();

      public const int BitsPerCell = 256;
      // 8x8 cells each requiring 4 bits. Total 256
      public int id { get; set; }
      [JsonIgnore]
      public Byte[] matterData {
         get; set;
      }
      public bool settled { get; set; }
      public bool entropic { get; set; }
      public byte chunkPosition { get; set; }

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

      public Cell() {
         matterData = new byte[BitsPerCell / 8];
         settled = true;
      }

      public Cell(byte x, byte y) : this() { SetChunkPosition(x, y); }

      public Cell(int x, int y) : this((byte)x, (byte)y) {}

      public Cell(String hex) : this() {
         for (int i = 0; i < hex.Length; i += 2) {
            string digit = hex.Substring(i, 2);
            byte byteValue = (byte)Convert.ToInt32(digit, 16);
            if (i == 0) {
               chunkPosition = byteValue;
            } else {
               matterData[i / 2 - 1] = byteValue;
            }
         }
      }

      public void Copy(Cell other) {
         this.matterData = (Byte[])other.matterData.Clone();
         this.settled = other.settled;
         this.entropic = other.entropic;
      }

      public void Write(Stream stream) {
         string pos = chunkPosition.ToString("X2");
         stream.WriteByte((byte)pos[0]);
         stream.WriteByte((byte)pos[1]);
         for (int i = 0; i < BitsPerCell / 8; i++) {
            string digit = matterData[i].ToString("X2");
            stream.WriteByte((byte)digit[0]);
            stream.WriteByte((byte)digit[1]);
         }
      }

      public Matter GetMatter(int x, int y) { return GetMatter((byte)x, (byte)y); }

      public Matter GetMatter(byte x, byte y) {
         if (x < 0 || x >= CellSize || y < 0 || y >= CellSize) {
            throw new Exception("Invalid Matter Get Index");
         }
         BitArray bitArray = new BitArray(matterData);
         byte bitIndex = (byte)(y * CellSize + x);
         byte matterValue = 0;
         for (byte i = 0; i < 4; i++) {
            if (bitArray[bitIndex * 4 + i]) {
               matterValue += (byte)Math.Pow(2, i);
            }
         }
         return (Matter)matterValue;
      }

      public void SetMatter(int x, int y, Matter matter) {
         if (x < 0 || x >= CellSize || y < 0 || y >= CellSize) {
            throw new Exception("Invalid Matter Get Index");
         }
         BitArray bitArray = new BitArray(matterData);
         byte[] matterByte = { (byte)matter };
         var newMatterData = new BitArray(matterByte);
         byte bitIndex = (byte)(y * CellSize + x);
         byte matterValue = 0;
         for (byte i = 0; i < 4; i++) {
            bitArray[bitIndex * 4 + i] = newMatterData[i];
         }
         bitArray.CopyTo(matterData, 0);
         if (matter.isSolid) {
            settled = false;
         }
      }

      public void Fill(Matter matter) {
         for (byte x = 0; x < CellSize; x++) {
            for (byte y = 0; y < CellSize; y++) {
               SetMatter(x, y, matter);
            }
         }
         settled = true;
      }

      public void FillRandom(Matter dominant = null) {
         for (byte x = 0; x < CellSize; x++) {
            for (byte y = 0; y < CellSize; y++) {
               SetMatter(x, y, Matter.Clay);
            }
         }
      }

      public byte GetMaxCellY() {
         byte[] cellYs = new byte[CellSize];
         for (byte x = 0; x < CellSize; x++) {
            cellYs[x] = 0;
            for (byte y = 1; y < CellSize; y++) {
               Matter matter = GetMatter(x, y);
               if (matter.isSolid) {
                  cellYs[x]++;
               }
            }
         }
         byte highestCellY = 0;
         for (byte i = 1; i < CellSize; i++) {
            if (cellYs[i] > cellYs[highestCellY]) {
               highestCellY = i;
            }
         }
         return cellYs[highestCellY];
      }

      public Matter GetMajority() {
         int[] matterCounts = new int[MatterTypes];
         for (byte x = 0; x < CellSize; x++) {
            for (byte y = 0; y < CellSize; y++) {
               Matter matter = GetMatter(x, y);
               matterCounts[(byte)matter]++;
            }
         }
         byte highestMatter = 0;
         for (byte i = 1; i < MatterTypes; i++) {
            if (matterCounts[i] > matterCounts[highestMatter]) {
               highestMatter = i;
            }
         }
         return (Matter)highestMatter;
      }

      public static void ParseOverlap(ref byte cellX, ref byte cellY, ref sbyte x, ref sbyte y) {
         sbyte cellSize = (sbyte)CellSize;
         if (y >= cellSize) {
            cellY++;
            y -= cellSize;
         } else if (y < 0) {
            cellY--;
            y += cellSize;
         }
         if (x >= cellSize) {
            cellX++;
            x -= cellSize;
         } else if (x < 0) {
            cellX--;
            x += cellSize;
         }
      }

      private static Matter GetMatter(Cell[,] neighbors, int x, int y) {
         return GetMatter(neighbors, (sbyte)x, (sbyte)y);
      }

      private static void SetMatter(Cell[,] neighbors, int x, int y, Matter matter) {
         SetMatter(neighbors, (sbyte)x, (sbyte)y, matter);
      }

      private static Matter GetMatter(Cell[,] neighbors, sbyte x, sbyte y) {
         byte cellX = 1;
         byte cellY = 1;
         ParseOverlap(ref cellX, ref cellY, ref x, ref y);
         if (neighbors[cellX, cellY] == null) {
            throw new MissingCellNeighborException();
         }
         return neighbors[cellX, cellY].GetMatter((byte)x, (byte)y);
      }

      private static bool IsEntropic(Cell[,] neighbors, int x, int y) {
         return IsEntropic(neighbors, (sbyte)x, (sbyte)y);
      }

      private static bool IsEntropic(Cell[,] neighbors, sbyte x, sbyte y) {
         byte cellX = 1;
         byte cellY = 1;
         ParseOverlap(ref cellX, ref cellY, ref x, ref y);
         if (neighbors[cellX, cellY] == null) {
            throw new MissingCellNeighborException();
         }
         return neighbors[cellX, cellY].entropic;
      }

      private static void SetMatter(Cell[,] neighbors, sbyte x, sbyte y, Matter matter) {
         byte cellX = 1;
         byte cellY = 1;
         ParseOverlap(ref cellX, ref cellY, ref x, ref y);
         if (neighbors[cellX, cellY] == null) {
            throw new MissingCellNeighborException();
         }
         if (neighbors[cellX, cellY].entropic) {
            return;
         }
         neighbors[cellX, cellY].SetMatter((byte)x, (byte)y, matter);
         if (y == 0 && cellY == 1 && matter.isSolid) {
            if (neighbors[cellX, 0] != null) {
               neighbors[cellX, 0].settled = false;
            }
         }
      }

      public bool IsSolid() {
         if (entropic) {
            return true;
         }
         return GetMajority().isSolid;
      }
   }
}
