% Clearing workspace and closing figures
close all; 
clear all; 
clc; 

% Reading the image
inputImage = imread('baymax.png');
figure, imshow(inputImage); 
title("Original Baymax"); 

% Converting RGB to LAB color space
labOutput = rgb2lab(inputImage); 

% Extracting lightness, a, and b channels
lightnessChannel = labOutput(:,:,1); 
aChannelOutput = labOutput(:,:,2); 
bChannelOutput = labOutput(:,:,3); 

% Calculating mean values for each channel
lightnessMeanValue = mean2(lightnessChannel); 
aMeanValue = mean2(aChannelOutput); 
bMeanValue = mean2(bChannelOutput);

% Calculating saliency map
saliencyMapOutput = (lightnessChannel - lightnessMeanValue).^2 + ... % Compute saliency map
             (aChannelOutput - aMeanValue).^2 + ...
             (bChannelOutput - bMeanValue).^2;

% Thresholding the saliency map
thresholdValue = mean(saliencyMapOutput(:)); 
saliencyMapOutput(saliencyMapOutput < thresholdValue) = 1; % Threshold to create binary map
saliencyMapOutput(saliencyMapOutput >= thresholdValue) = 0;

% Applying saliency map
for channelIndex = 1:3
  outputImage(:,:,channelIndex) = inputImage(:,:,channelIndex) .* uint8(saliencyMapOutput); % Apply binary mask to each channel
end

% Displaying the output image
figure, imshow(outputImage); 
title("Extracted Baymax"); 
