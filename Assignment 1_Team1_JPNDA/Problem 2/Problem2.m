% Close all figure windows
close all;

% Clear workspace variables
clear all;

% Clear command window
clc;

% Read the color image
img = imread('singapore.jpg');

%Convert the image to double precision for processing
img = im2double(img);

% Define the Gaussian filter    
gaussian_kernel = fspecial('gaussian', [25 25],5);

% Apply the Gaussian filter to the color image
img_gaussian = imfilter(img, gaussian_kernel, 'replicate');

% Sharpen the image by subtracting the blurred image from the original
img_sharpened = img * 2 - img_gaussian;

% Display the original and the sharpened image
figure;imshow(img);
title('Original Image');


figure,imshow(img_sharpened);
title('Sharpened Image');

