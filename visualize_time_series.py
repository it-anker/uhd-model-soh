import json
import sys
import matplotlib.pyplot as plt
from datetime import datetime

def plot_line_chart(file_path):
    # Open file and load JSON data
    with open(file_path, 'r') as file:
        data = json.load(file)

    # Extract data
    date_times = [datetime.fromisoformat(entry['dateTime']) for entry in data]
    values = [entry['value'] for entry in data]

    # Create line chart
    plt.figure(figsize=(10, 6))
    plt.plot(date_times, values, marker='o', linestyle='-', color='b', label='Value')

    # Chart labels and title
    plt.xlabel('Time', fontsize=12)
    plt.ylabel('Value', fontsize=12)
    plt.title('Line Chart of Values Over Time', fontsize=14)
    plt.grid(True)
    plt.legend()

    # X-Achsen-Labels anpassen
    plt.xticks(rotation=45, fontsize=10)
    plt.tight_layout()

    # Diagramm anzeigen
    plt.show()

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python script.py <path_to_json_file>")
        sys.exit(1)

    file_path = sys.argv[1]
    plot_line_chart(file_path)
