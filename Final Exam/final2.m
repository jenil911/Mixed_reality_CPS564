% Read the input images
ironManImage = imread('iron_man_green.jpg'); % Image with Iron Man and green background
backgroundImage = imread('zoom_background.jpg'); % New background image

% Convert the images to double precision for processing
ironManImage = double(ironManImage); % Convert Iron Man image to double
backgroundImage = double(backgroundImage); % Convert background image to double

% Convert to HSV color space for easier color segmentation
ironManHSV = rgb2hsv(ironManImage); % Convert Iron Man image to HSV color space

% Define the green color range for masking
hueMin = 0.2; % Minimum hue value for green
hueMax = 0.4; % Maximum hue value for green
saturationMin = 0.2; % Minimum saturation for the green color
valueMin = 0.2; % Minimum value for the green color

% Create the mask for green color
greenMask = (ironManHSV(:,:,1) >= hueMin & ironManHSV(:,:,1) <= hueMax) & ...
             (ironManHSV(:,:,2) >= saturationMin) & ...
             (ironManHSV(:,:,3) >= valueMin); % Mask where the green color is present

% Convert the mask to logical type
greenMask = logical(greenMask); % Convert mask to logical type (true/false)

% Resize the background image to match the size of Iron Man image
backgroundImage = imresize(backgroundImage, [size(ironManImage, 1), size(ironManImage, 2)]); % Resize background

% Create the output image
outputImage = zeros(size(ironManImage), 'double'); % Initialize output image

% Replace the green screen with the new background
outputImage = backgroundImage; % Start with the background image
outputImage(repmat(~greenMask, [1 1 3])) = ironManImage(repmat(~greenMask, [1 1 3])); % Replace non-green areas with Iron Man image

% Convert the output image to uint8 for display
outputImage = uint8(outputImage); % Convert to uint8 for proper display

% Display the result
figure;
imshow(outputImage); % Show the final image
title('Iron Man with New Background'); % Title for the displayed image


