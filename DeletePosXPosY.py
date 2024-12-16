import csv


def remove_columns(input_file, output_file):
    with open(input_file, 'r', encoding='utf-8') as infile, open(output_file, 'w', encoding='utf-8',
                                                                 newline='') as outfile:
        reader = csv.reader(infile)
        writer = csv.writer(outfile)

        for row in reader:
            # Keep only the relevant columns: gaze_X, gaze_Y, fixationTime
            # Assuming the column order: gaze_X (0), gaze_Y (1), pos_X (2), pos_Y (3), fixationTime (4)
            filtered_row = [row[0], row[1], row[4]]
            writer.writerow(filtered_row)


# Example usage
input_file = 'input.csv'
output_file = 'output.csv'

remove_columns(input_file, output_file)
