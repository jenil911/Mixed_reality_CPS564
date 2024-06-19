% Set MATLAB to a known state.
close all;
clear all;
clc;

img = imread('singapore.jpg');
figure('Name', 'Original Image'), imshow(img);
[rows, cols, depth] = size(img);

if (depth > 1)
    % If the depth> 1 it is assumed to be an RGB image
    img_gray = rgb2gray(img);
    figure('Name', 'Grayscale Image'), imshow(img_gray);
else
    % If the image is already a grayscale image just use it with no conversion
    img_gray = img;
end

% Perform a min filter with a neighboring region size of 7
filter_size = 7;
min_img = filterImage(filter_size, img_gray, "min");
figure('Name', ['Min Filter: Neighboring region size = ' num2str(filter_size)]), imshow(min_img);

% Perform a max filter with a neighboring region size of 5
filter_size = 5;
max_img = filterImage(filter_size, img_gray, "max");
figure('Name', ['Max Filter: Neighboring region size = ' num2str(filter_size)]), imshow(max_img);

% Perform a median filter with a neighboring region size of 3
filter_size = 3;
median_img = filterImage(filter_size, img_gray, "median");
figure('Name', ['Median Filter: Neighboring region size = ' num2str(filter_size)]), imshow(median_img);


function flt_img = filterImage(filter_size, img_gray, filter_type)
% filterImage performs the specified filter with the specified size on the
% given grayscale image.  The resulting filtered image is returned.
%    filter_size is the size of the neighboring region in pixels and must
%                be an odd number.
%    img_gray    is a grayscale image of type uint8.
%    filter_type is the type of filter to be performed: "min", "max", or
%                "median".
    [rows, cols, depth] = size(img_gray);
    flt_img = zeros(rows, cols, 'uint8');

    % Verify that the filter_size is odd
    if (mod(filter_size,2) == 0)
        disp("Filter size must be an odd number.  Aborting filter.")
    else
        % Verify that the image is grayscale
        if (depth > 1)
            disp("Image is not grayscale.  Aborting filter.")
        else
            indent = (filter_size - 1) / 2;

            % Determine if a max filter is required
            if (filter_type == "max")
                % Loop through each row of the input image
                for row=indent+1:rows-indent
                    % Loop through each column of the input image
                    for col = indent+1:cols-indent
                        neighbors = img_gray(row-indent:row+indent,col-indent:col+indent);
                        % Use the built-in MATLAB max function for speed
                        % since it is compiled code rather than interpreted.
                        max_val = max(neighbors, [], "all");
                        flt_img(row, col) = max_val;
                    end
                end

            % Determine if a min filter is required
            elseif (filter_type == "min")
                % Loop through each row of the input image
                for row=indent+1:rows-indent
                    % Loop through each column of the input image
                    for col = indent+1:cols-indent
                        neighbors = img_gray(row-indent:row+indent,col-indent:col+indent);
                        % Use the built-in MATLAB min function for speed
                        % since it is compiled code rather than interpreted.
                        min_val = min(neighbors, [], "all");
                        flt_img(row, col) = min_val;
                    end
                end

            % Determine if a median filter is required
            elseif (filter_type == "median")
                % Loop through each row of the input image
                for row=indent+1:rows-indent
                    % Loop through each column of the input image
                    for col = indent+1:cols-indent
                        neighbors = img_gray(row-indent:row+indent,col-indent:col+indent);
                        % Use the built-in MATLAB median function for speed
                        % since it is compiled code rather than interpreted.
                        med_val= median(neighbors, "all");
                        flt_img(row, col) = med_val;
                    end
                end
                            % Determine if an alpha-trimmed mean filter is required
            elseif (filter_type == "alpha_trimmed")
                alpha = 2;
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

            % If any other filter is requested abort
            else
                disp("Invalid filter type """ + filter_type + """.  Valid values are ""min"", ""max"", or ""median"".");
            end
        end
    end
end