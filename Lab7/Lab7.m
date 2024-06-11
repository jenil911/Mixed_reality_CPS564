clear all;
close all;
clc;

% Load the image
img = imread('deer.jpg');
figure, imshow(img);

% Apply Gaussian Filter
gaussian_kernel = fspecial('gaussian', [25 25], 5);
img_gaussian = imfilter(img, gaussian_kernel, 'replicate');
figure, imshow(img_gaussian);

% Convert to Lab
lab = rgb2lab(img_gaussian);

% Compute Mean for each channel
l = double(lab(:,:,1));
lm = mean(mean(l));
a = double(lab(:,:,2));
am = mean(mean(a));
b = double(lab(:,:,3));
bm = mean(mean(b));

% Compute the Saliency
sm = (l-lm).^2 + (a-am).^2 + (b-bm).^2;
figure, imshow(sm,[]);

mean_value = mean(sm(:));
sm(sm < mean_value) = 0;
sm(sm >= mean_value) = 1;
figure, imshow(sm,[]);

for c = 1:3
    img(:,:,c) = img(:,:,c) .* uint8(sm);
end
figure, imshow(img);






