% Read a grayscale image (or convert a color image to grayscale)
img = imread('singapore.jpg');  
if size(img, 3) == 3
    img = rgb2gray(img); 
end

% Compute the importance map
importanceMap = Importance(img);

% Display the original grayscale image and the computed importance map
figure;
subplot(1, 2, 1);  
imshow(img);  % Show the original grayscale image
title('Original Grayscale Image');  

subplot(1, 2, 2);  % Switch to the second plot
imshow(importanceMap, []);  %
title('Importance Map');  
