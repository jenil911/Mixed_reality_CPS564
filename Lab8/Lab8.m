close all;
clear all;
clc;

img = imread("");
figure, imshow(img);

sm = saliency(img);
figure, imshow(sm,[]);

[height, width] = size(sm);
new_height = height*2/3;
new_width = width*2/3;

height_values = sum(sm');
width_values = sum(sm);

[~,ind1] = sort(height_values);
[~,ind2] = sort(width_values);

img(ind1(1:height - new_height),:,:) = [];
figure, imshow(img);

img(:,ind2(1:width - new_width),:) = [];
figure, imshow(img);