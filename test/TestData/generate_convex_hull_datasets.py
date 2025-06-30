import numpy as np
from scipy.spatial import ConvexHull
import os

# Počty bodů, které chceme vygenerovat
sizes = [1500, 5000, 65000]

# Výstupní složka
output_dir = "datasets"
os.makedirs(output_dir, exist_ok=True)

for size in sizes:
    # Generování náhodných bodů v rozsahu [0, 1)
    points = np.random.rand(size, 2)

    # Výpočet konvexní obálky
    hull = ConvexHull(points)
    indices = hull.vertices
    full_indices = points[indices]

    # Soubory pro tuto velikost
    prefix = f"{size}_points"
    points_file = os.path.join(output_dir, f"{prefix}.csv")
    indices_file = os.path.join(output_dir, f"{prefix}_indices.txt")
    full_indices_file = os.path.join(output_dir, f"{prefix}_full_indices.csv")

    # Uložení bodů do CSV (X;Y)
    with open(points_file, "w", encoding="utf-8") as f:
        for point in points:
            f.write(f"{point[0]:.6f};{point[1]:.6f}\n")

    # Uložení indexů
    with open(indices_file, "w", encoding="utf-8") as f:
        for idx in indices:
            f.write(f"{idx}\n")

    # Uložení souřadnic bodů konvexní obálky
    with open(full_indices_file, "w", encoding="utf-8") as f:
        for point in full_indices:
            f.write(f"{point[0]:.6f};{point[1]:.6f}\n")

    print(f"✅ Vygenerováno: {prefix} ({size} bodů)")

print("\nHotovo. Soubory najdeš ve složce ./datasets")
