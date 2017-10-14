from __future__ import division
import itertools
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import matplotlib as mpl
from sklearn.decomposition import PCA
from sklearn import mixture
from scipy import linalg
from math import ceil, sqrt
from sklearn.cluster import AgglomerativeClustering, MeanShift, DBSCAN

from os import listdir
from os.path import isfile, join

color_iter = ['crimson', 'c', 'green', 'gold', 'darkorange', 'darkblue',
              'b', 'red', 'green', 'k', 'y', 'olive', 'crimson',
              'brown']

result_file_path = r'C:\Users\Forczu\Documents\Visual Studio 2015\Projects\MaConsole\MaConsole\bin\Release\Results\Mutation'
output_file_path = r'H:\Mutation_without_ac_1.txt'


class LandscapeScanResult:
    def __init__(self, type, number, instance, operator, ac, ic, pic, dbi):
        self.type = type
        self.number = number
        self.instance = instance
        self.operator = operator
        self.ac = np.mean(ac)
        self.ic = np.mean(ic)
        self.pic = np.mean(pic)
        self.dbi = np.mean(dbi)

    def to_array(self):
        return [self.type, self.number, self.instance, self.operator, self.ac, self.ic, self.pic, self.dbi]


class ClusterData:
    def __init__(self, number):
        self.number = number
        self.elements = []

    def size(self):
        return len(self.elements)

    def mean_ac(self):
        return get_mean(self.elements, 'AC')

    def mean_ic(self):
        return get_mean(self.elements, 'IC')

    def mean_pic(self):
        return get_mean(self.elements, 'PIC')

    def mean_dbi(self):
        return get_mean(self.elements, 'DBI')


def get_mean(rows, column_name):
    sum = 0
    for r in rows:
        sum += r[column_name]
    return sum / len(rows)


def parse_csv_files(path):
    results = []
    result_files = [f for f in listdir(path) if isfile(join(path, f))]
    for file in result_files:
        filenameParts = file.split('_')
        type = filenameParts[0]
        number = filenameParts[1]
        instance = filenameParts[2]
        operator = filenameParts[3].split('.')[0]
        filePath = path + r'\\' + file
        result_df = pd.read_csv(filePath, skipinitialspace=True)
        ac = result_df.as_matrix(['AC'])
        ic = result_df.as_matrix(['IC'])
        pic = result_df.as_matrix(['PIC'])
        dbi = result_df.as_matrix(['DBI'])
        result = LandscapeScanResult(type, number, instance, operator, ac, ic, pic, dbi)
        results.append(result)
    return results


def create_mean_results_dataframe(results):
    columns = ['Type', 'Number', 'Instance', 'Operator', 'AC', 'IC', 'PIC', 'DBI']
    index = np.arange(0, len(results))
    results_as_arrays = [r.to_array() for r in results]
    data = np.array(results_as_arrays)
    df = pd.DataFrame(data, columns=columns, index=index)
    df[['AC', 'IC', 'PIC', 'DBI']] = df[['AC', 'IC', 'PIC', 'DBI']].astype(float)
    return df


def create_em_mixture(data, components_number):
    return mixture.GaussianMixture(n_components=components_number, covariance_type='full',
                                   init_params='kmeans', tol=1e-2).fit(data)


def find_best_cluster_number(df, min_cluster_number, max_clusters_number):
    bics = []
    #data = df._get_numeric_data().as_matrix()
    data = df
    for i in range(min_cluster_number, max_clusters_number + 1):
        gmm = create_em_mixture(data, i)
        bic = gmm.bic(data)
        bics.append(bic)
    maxVal = max(bics)
    return bics.index(maxVal) + min_cluster_number;


def plot_results(X, Y_, means, covariances, index, title):
    splot = plt.subplot(1, 1, 1)
    for i, (mean, covar, color) in enumerate(zip(means, covariances, color_iter)):
        v, w = linalg.eigh(covar)
        v = 2. * np.sqrt(2.) * np.sqrt(v)
        u = w[0] / linalg.norm(w[0])
        if not np.any(Y_ == i):
            continue
        plt.scatter(x=X[Y_ == i, 0], y=X[Y_ == i, 1], s=.8, color=color)

        # Plot an ellipse to show the Gaussian component
        angle = np.arctan(u[1] / u[0])
        angle = 180. * angle / np.pi  # convert to degrees
        ell = mpl.patches.Ellipse(mean, v[0], v[1], 180. + angle, color=color)
        ell.set_clip_box(splot.bbox)
        ell.set_alpha(0.5)
        splot.add_artist(ell)
    plt.title(title)


def clustering_plot(pca, means, covariances, operators, predicted):
    splot = plt.subplot(1, 1, 1)
    for i, (mean, covar) in enumerate(zip(means, covariances)):
        v, w = linalg.eigh(covar)
        v = 2. * np.sqrt(2.) * np.sqrt(v)
        u = w[0] / linalg.norm(w[0])
        if not np.any(predicted == i):
            continue
        for j in range(len(predicted)):
            if predicted[j] == i:
                marker = '$\\mathrm{' + operators[j][0] + '}$'
                plt.scatter(x=pca[j, 0], y=pca[j, 1], s=70, color=color_iter[i], marker=marker)

        angle = np.arctan(u[1] / u[0])
        angle = 180. * angle / np.pi  # convert to degrees
        ell = mpl.patches.Ellipse(mean, v[0], v[1], 180. + angle, color=color_iter[i])
        ell.set_clip_box(splot.bbox)
        ell.set_alpha(0.3)
        splot.add_artist(ell)


def main():
    researchResults = parse_csv_files(result_file_path)
    df = create_mean_results_dataframe(researchResults)

    df = df.drop('AC', 1)
    numeric_df = df._get_numeric_data()
    pca_2 = PCA(2)
    plot_columns = pca_2.fit_transform(numeric_df)

    is_gmm = True

    optimal_cluster_number = find_best_cluster_number(plot_columns, 4, ceil(sqrt(len(df.index))))
    if is_gmm:
        cluster_number = optimal_cluster_number
        gmm = create_em_mixture(plot_columns, optimal_cluster_number)
    else:
        cluster_number = 4
        gmm = mixture.BayesianGaussianMixture(n_components=4, covariance_type='full',
                                              weight_concentration_prior_type='dirichlet_process').fit(plot_columns)

    ward = AgglomerativeClustering(n_clusters=cluster_number, linkage='ward').fit(plot_columns)
    predicted = gmm.predict(plot_columns)

    df['Cluster'] = predicted
    clustering_plot(plot_columns, gmm.means_, gmm.covariances_, df['Operator'], predicted)

    clusters = [ClusterData(i + 1) for i in range(len(predicted))]
    for index, row in df.iterrows():
        clusters[row['Cluster']].elements.append(row)

    file = open(output_file_path, "w")
    file.write('Type ')
    for i, val in enumerate(clusters):
        if not val.elements:
            continue
        file.write(str(val.number))
        file.write('\n')
        for j, row in enumerate(val.elements):
            for k, cell in row.iteritems():
                file.write(str(cell))
                file.write(' ')
            file.write('\n')
        file.write('\n')
    file.close()

    plt.show()


if __name__ == "__main__":
    main()
