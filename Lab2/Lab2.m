clear all;
close all;
clc;

img = imread('singapore.jpg');

figure, imshow(img);

img_gray = rgb2gray(img);

figure ,imshow(img_gray);

img_gray(1:10,1:end) = 0;

figure ,imshow(img_gray);

img_gray(1:end,1:10) = 0;

figure ,imshow(img_gray);

% Display the histogram of the grayscale image
figure, histogram(img_gray);

% Perform histogram equalization
img_eq = histeq(img_gray);

% Display the histogram of the equalized image
figure, histogram(img_eq);

% Show the equalized image
figure, imshow(img_eq);