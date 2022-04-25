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
using System.Linq;

namespace boundaries.Models {

   public class Chunk {
      public const byte ChunkSize = Cell.CellSize;

      public int x { get; set; }
      public int y { get; set; }
      public List<Cell> cells { get; set; }
      public List<Entropic> entropics { get; set; }
      public int cacheCellId { get; set; }
      public bool isActive { get; set; }
      public bool generated { get; set; }

      public Chunk(int x, int y) {
         this.x = x;
         this.y = y;
         cells = new List<Cell>();
         cacheCellId = 0;
         isActive = true;
         entropics = new List<Entropic>();
         generated = false;
      }

      public Cell GetCellLocal(int x, int y) { return GetCellLocal((byte)x, (byte)y); }

      public Cell GetCellLocal(byte x, byte y) {
         if (x < 0 || x >= ChunkSize) {
            return null;
         }
         if (y < 0 || y >= ChunkSize) {
            return null;
         }
         return cells.Where(c => c.x == x && c.y == y).FirstOrDefault();
      }

      public static Cell GetCellLocal(Chunk[] neighbors, Cell[] caches, int x, int y) {
         return GetCellLocal(neighbors, caches, (sbyte)x, (sbyte)y);
      }

      public static void SetIsActive(Chunk[] neighbors, sbyte x, sbyte y) {
         byte chunkX = 1;
         if (x >= ChunkSize) {
            chunkX++;
            x -= (sbyte)ChunkSize;
         } else if (x < 0) {
            chunkX--;
            x += (sbyte)ChunkSize;
         }
         if (neighbors[chunkX] != null) {
            neighbors[chunkX].isActive = true;
         }
      }

      public static Cell GetCellLocal(Chunk[] neighbors, Cell[] caches, sbyte x, sbyte y) {
         byte chunkX = 1;
         if (x >= ChunkSize) {
            chunkX++;
            x -= (sbyte)ChunkSize;
         } else if (x < 0) {
            chunkX--;
            x += (sbyte)ChunkSize;
         }
         if (neighbors[chunkX] == null) {
            return null;
         }
         Cell returnCell = neighbors[chunkX].GetCellLocal((byte)x, (byte)y);
         if (returnCell == null && y >= 0 && y < ChunkSize && caches[chunkX] != null &&
             neighbors[chunkX].generated) {
            returnCell = new Cell(x, y);
            returnCell.id = -1;
            Matter cacheMatter = caches[chunkX].GetMatter(x, y);
            returnCell.Fill(cacheMatter);
            returnCell.settled = false;
            // neighbors[chunkX].cells.Add(returnCell);
         }
         return returnCell;
      }

      public void SetCacheCell(Cell cacheCell) {
         cacheCellId = cacheCell.id;
         UpdateCache(cacheCell);
      }

      public void UpdateCache(Cell cacheCell) {
         // if (cacheCell.id != 0) {
         // Console.WriteLine("updating cache at " + cacheCell.id);
         // }
         // for (byte cellX = 0; cellX < ChunkSize; cellX++) {
         //    Cell surfaceCell = GetSurfaceLocal(cellX);
         //    if (surfaceCell != null) {
         //       cacheCell.SetMatter(cellX, surfaceCell.y, surfaceCell.GetMajority());
         //    }
         // }
         for (byte cellX = 0; cellX < ChunkSize; cellX++) {
            for (byte cellY = 0; cellY < ChunkSize; cellY++) {
               Cell cell = GetCellLocal(cellX, cellY);
               if (cell != null) {
                  cacheCell.SetMatter(cellX, cellY, cell.GetMajority());
               }
            }
         }
      }

      public Entropic GetEntropic(int x, int y) {
         foreach (Entropic entropic in entropics) {
            if (entropic.GetLocation(x, y)) {
               return entropic;
            }
         }
         return null;
      }

      public Cell GetSurfaceLocal(int x) {
         Cell terrainCell = null;
         for (int i = 7; i >= 0; i--) {
            terrainCell = GetCellLocal(x, i);
            if (terrainCell != null) {
               if (terrainCell.IsSolid() && !terrainCell.entropic) {
                  break;
               }
            }
         }
         return terrainCell;
      }

      public Entropic GetSurfaceEntropic(int x) {
         Entropic surfaceEnt = null;
         for (int i = 7; i >= 0; i--) {
            surfaceEnt = GetEntropic(x, i);
            if (surfaceEnt != null) {
               break;
            }
         }
         return surfaceEnt;
      }
   }
}
