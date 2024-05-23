close all;
clear all;
clc;

img = imread('singapore.jpg');
figure, imshow(img);

img_gray = rgb2gray(img);
figure, imshow(img_gray);

kernel = [-1 0 1; -2 0 2; -1 0 1];

img_gray_sobel = imfilter(img_gray, kernel, 'replicate');
figure, imshow(img_gray_sobel);

img_sobel = imfilter(img, kernel, 'replicate');
figure, imshow(img_sobel);

gaussian_kernel = fspecial('gaussian', [25 25], 5);
img_gray_gaussian = imfilter(img_gray, gaussian_kernel,'replicate');
figure, imshow(img_gray_gaussian);

figure,imshow(gaussian_kernel,[]);

img_sharpened = img_gray * 2 - img_gray_gaussian;
figure, imshow(img_sharpened);

img_sharpened2 = double(img_gray) * 2 - double(img_gray_gaussian);
figure, imshow(img_sharpened2);

img_sharpened3 = uint8(img_sharpened2);
figure, imshow(img_sharpened3);

img_noise = imread('salt_and_pepper.jpg');
figure, imshow(img_noise);

img_denoise = medfilt2(img_noise);
figure,imshow(img_denoise);

img_denoise = medfilt2(img_noise,[5 5]);
figure,imshow(img_denoise);

