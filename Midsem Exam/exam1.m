% Clear variables, close figures, and clear command window
clear all;
close all;
clc;

% Read the mountain image
img = imread('mountain.jpg');
figure, imshow(img);
title("original Image");

% Define Gaussian filter kernel as instructed in the problem
gaussian_kernel = fspecial('gaussian', [5 5], 1); 

% Apply Gaussian filter to smooth the image as it is mentioned
img_smooth = imfilter(img, gaussian_kernel, 'replicate');
figure, imshow(img_smooth);
title("Gaussian Image");

% using 1.5 to submit and 0.5 and 1 for other questions
alpha = 2; 

% Sharpen the image using the formula: it can be seen in the question 
img_sharp = (1 + alpha) * double(img) - alpha * double(img_smooth);

% Convert the sharpened image back to uint8 otherwise no good result
img_sharp = uint8(img_sharp);

% Display the final sharpened image
figure, imshow(img_sharp);
title("Sharpned Alpha 2")