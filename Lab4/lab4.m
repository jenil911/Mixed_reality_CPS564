close all;
clear all;
clc;

img = imread('mountain.png');
img_gray = rgb2gray(img);
kernel_size = 5;
gaussian_kernel = fspecial('gaussian', [kernel_size kernel_size], 5);
img_gray_gaussian = imfilter(img_gray, gaussian_kernel, 'replicate');
figure, imshow(img_gray_gaussian);

% Preparation for BF
indent = (kernel_size - 1)/2;
[height, width] = size(img_gray);
img_results = zeros(height,width);
img_gray = double(img_gray);
sigma_range = 5;

% Compute joint kernel
for i = indent + 1:height - indent
    for j = indent + 1:width - indent
        range_kernel = exp(-abs(img_gray(i - indent:i + indent,j - indent:j + indent )- img_gray(i,j)).^2/(sigma_range * sigma_range));
        kernel = range_kernel .* gaussian_kernel;
        normalization = 1/sum(kernel(:));
        temp = (kernel.*double(img_gray(i - indent:i + indent,j - indent:j + indent))) *normalization;
        img_results(i,j) = sum(temp(:));
    end
end

figure, imshow(img_results,[]);






