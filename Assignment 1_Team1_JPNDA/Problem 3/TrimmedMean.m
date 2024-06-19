    % Complete code to demonstrate the alpha-trimmed mean filter

% Close all figure windows
close all;

% Clear workspace variables
clear all;

% Clear command window
clc;

% Read the grayscale image
img = imread('salt_and_pepper.jpg'); % Replace with your image file name
figure('Name', 'Original Image'), imshow(img);
[rows, cols, depth] = size(img);

% Convert to grayscale if the image is RGB
if (depth > 1)
    img_gray = rgb2gray(img);
    figure('Name', 'Grayscale Image'), imshow(img_gray);
else
    img_gray = img;
end

% Perform an alpha-trimmed mean filter with a neighboring region size of 3
filter_size = 3;
alpha = 2;
alpha_trimmed_img = filterImage(filter_size, img_gray, "alpha_trimmed", alpha);
figure('Name', ['Alpha-Trimmed Mean Filter: Neighboring region size = ' num2str(filter_size)]), imshow(alpha_trimmed_img);

function flt_img = filterImage(filter_size, img_gray, filter_type, alpha)
    % filterImage performs the specified filter with the specified size on the
    % given grayscale image. The resulting filtered image is returned.
    % filter_size is the size of the neighboring region in pixels and must
    % be an odd number.
    % img_gray is a grayscale image of type uint8.
    % filter_type is the type of filter to be performed: "min", "max", "median", or "alpha_trimmed".
    % alpha is the number of pixels to trim for the alpha-trimmed mean filter

    [rows, cols, depth] = size(img_gray);
    flt_img = zeros(rows, cols, 'uint8');

    % Verify that the filter_size is odd
    if (mod(filter_size, 2) == 0)
        disp("Filter size must be an odd number. Aborting filter.")
    else
        % Verify that the image is grayscale
        if (depth > 1)
            disp("Image is not grayscale. Aborting filter.")
        else
            indent = (filter_size - 1) / 2;

            % Determine if an alpha-trimmed mean filter is required
            if (filter_type == "alpha_trimmed")
                % Loop through each row of the input image
                for row = indent+1:rows-indent
                    % Loop through each column of the input image
                    for col = indent+1:cols-indent
                        neighbors = img_gray(row-indent:row+indent, col-indent:col+indent);
                        % Sort the neighbors
                        sorted_neighbors = sort(neighbors(:));
                        % Remove the alpha highest and lowest values
                        trimmed_neighbors = sorted_neighbors(alpha+1:end-alpha);
                        % Compute the mean of the remaining values
                        mean_val = mean(trimmed_neighbors);
                        flt_img(row, col) = mean_val;
                    end
                end

            % If any other filter is requested, abort
            else
                disp("Invalid filter type """ + filter_type + """. Valid values are ""alpha_trimmed"".");
            end
        end
    end
end
